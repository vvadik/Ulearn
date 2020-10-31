from subprocess import Popen, PIPE, TimeoutExpired
from os import listdir
import json
from SourceCodeRunInfo import get_run_info_by_language_name, ISourceCodeRunInfo
from verdict import Verdict


class CompilationException(Exception):
    pass


class RuntimeException(Exception):
    pass


class TimeLimitException(Exception):
    pass


class WrongAnswerException(Exception):
    pass


class TaskCodeRunner:
    def __init__(self, source_code_run_info: ISourceCodeRunInfo):
        self.__source_code_run_info = source_code_run_info
        self.__runnable_file_done = False
        self.__runnable_file = None

    def build(self, code_filename: str, result_filename: str):
        command = self.__source_code_run_info.format_build_command(code_filename, result_filename).split()
        process = Popen(command)
        _, err = process.communicate()
        if process.returncode != 0:
            raise CompilationException(err)

    def run(self, test_file: str, result_file: str, time_limit: int):
        command = self.__source_code_run_info.format_run_command(self.__runnable_file).split()
        process = Popen(command, stdin=open(test_file, 'r'), stdout=open(result_file, 'w'), stderr=PIPE)
        try:
            process.wait(time_limit)
        except TimeoutExpired:
            raise TimeLimitException()

        if process.returncode != 0:
            raise RuntimeException()

    def run_test(self, code_filename: str, test_file: str, time_limit: int):
        if self.__runnable_file is None:
            if self.__source_code_run_info.need_build():
                self.__runnable_file = 'tmp'
                self.build(code_filename, self.__runnable_file)
            else:
                self.__runnable_file = code_filename

        self.run(test_file, f'result', time_limit)


def load_settings(settings_filename: str):
    with open(settings_filename, 'r') as settings_file:
        return json.load(settings_file)


def check(code_filename):
    number_test = 1
    source_code_run_info = get_run_info_by_language_name('python')
    runner = TaskCodeRunner(source_code_run_info)
    for test in sorted(listdir('tests')):
        if test.endswith('.a'):
            continue
        try:
            runner.run_test(code_filename, f'tests/{test}', 1)
            process = Popen(['./check', f'tests/{test}', 'result', f'tests/{test}.a'], stdout=PIPE, stderr=PIPE)
            out, err = process.communicate()
            if err.startswith(b'ok'):
                number_test += 1
            else:
                raise WrongAnswerException()
        except CompilationException:
            return {
                'Verdict': Verdict.CompilationError.name
            }
        except RuntimeException:
            return {
                'Verdict': Verdict.RuntimeError.name,
                'TestNumber': number_test
            }
        except TimeLimitException:
            return {
                'Verdict': Verdict.TimeLimit.name,
                'TestNumber': number_test
            }
        except WrongAnswerException:
            return {
                'Verdict': Verdict.WrongAnswer.name,
                'TestNumber': number_test
            }
    return {
        'Verdict': Verdict.Ok.name,
    }


def main(settings_filename):
    # settings = load_settings(settings_filename)
    runner = TaskCodeRunner(get_run_info_by_language_name('cpp'))
    runner.build('check.cpp', 'check')

    print(check('solution.py'))


if __name__ == '__main__':
    main('settings.json')
