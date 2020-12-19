from subprocess import Popen, PIPE, TimeoutExpired
from os import listdir, rename
from os.path import join as path_join
from sys import argv

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
        command = self.__source_code_run_info.format_run_command(self.__runnable_file)
        with open(test_file, 'rb') as f:
            test_data = f.read()
        process = Popen(['su', STUDENT_USER, '-c', f"{command}"],
                        stdin=PIPE, stdout=open(result_file, 'wb'), stderr=PIPE)
        try:
            err = process.communicate(test_data, timeout=time_limit)
        except TimeoutExpired:
            raise TimeLimitException()

        if process.returncode != 0:
            if b'PermissionError' in err[1]:
                raise SecurityException()
            raise RuntimeException()

    def run_test(self, code_filename: str, test_file: str):
        if self.__runnable_file is None:
            if self.__source_code_run_info.need_build():
                self.__runnable_file = TEMPORARY_FILENAME
                self.build(code_filename, self.__runnable_file)
            else:
                self.__runnable_file = code_filename

        self.run(test_file, RESULT_FILENAME)


def check(source_code_run_info, code_filename):
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
        except SecurityException:
            return {
                'Verdict': Verdict.SecurityException.name
            }
    return {
        'Verdict': Verdict.Ok.name,
    }


def main():
    runner = TaskCodeRunner(get_run_info_by_language_name('cpp'))
    runner.build('check.cpp', 'check')
    run_info = get_run_info_by_language_name(language)
    solution_filename_by_language = SOLUTION_FILENAME.replace('.any', run_info.file_extension())
    rename(SOLUTION_FILENAME, solution_filename_by_language)
    result = check(run_info, solution_filename_by_language)
    print(result)

language = argv[1].lower()
time_limit = int(argv[2]) / 1000

if __name__ == '__main__':
    main()
