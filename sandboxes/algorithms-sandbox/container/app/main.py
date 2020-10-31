from subprocess import Popen, PIPE, TimeoutExpired
from os import listdir
from os.path import join as path_join
import json

from SourceCodeRunInfo import get_run_info_by_language_name, ISourceCodeRunInfo
from verdict import Verdict
from constants import *
from exceptions import *


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

    def run(self, test_file: str, result_file: str):
        command = self.__source_code_run_info.format_run_command(self.__runnable_file).split()
        process = Popen(command, stdin=open(test_file), stdout=open(result_file, 'w'), stderr=PIPE)
        try:
            process.wait(time_limit)
        except TimeoutExpired:
            raise TimeLimitException()

        if process.returncode != 0:
            raise RuntimeException()

    def run_test(self, code_filename: str, test_file: str):
        if self.__runnable_file is None:
            if self.__source_code_run_info.need_build():
                self.__runnable_file = TEMPORARY_FILENAME
                self.build(code_filename, self.__runnable_file)
            else:
                self.__runnable_file = code_filename

        self.run(test_file, RESULT_FILENAME)


def load_settings(settings_filename: str):
    with open(settings_filename) as settings_file:
        return json.load(settings_file)


def check(code_filename):
    source_code_run_info = get_run_info_by_language_name(language)
    runner = TaskCodeRunner(source_code_run_info)
    for number_test, test_filename in enumerate(sorted(listdir(TEST_DIRECTORY)), 1):
        if test_filename.endswith(SUFFIX_ANSWER_FILENAME):
            continue
        try:
            runner.run_test(code_filename, path_join(TEST_DIRECTORY, test_filename))
            process = Popen([path_join('.', 'check'),
                             path_join(TEST_DIRECTORY, test_filename),
                             RESULT_FILENAME,
                             path_join(TEST_DIRECTORY, f'{test_filename}{SUFFIX_ANSWER_FILENAME}')],
                            stdout=PIPE, stderr=PIPE)
            _, err = process.communicate()
            if not err.startswith(b'ok'):
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


def main():
    runner = TaskCodeRunner(get_run_info_by_language_name('cpp'))
    runner.build('check.cpp', 'check')

    result = check(SOLUTION_FILENAME)
    print(result)


settings = load_settings(SETTINGS_FILENAME)
time_limit = settings["TimeLimit"]
language = settings["Language"]

if __name__ == '__main__':
    main()
