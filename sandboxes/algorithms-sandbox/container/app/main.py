from subprocess import Popen, PIPE, TimeoutExpired, DEVNULL
from os import listdir, rename
import shutil
from os.path import join as path_join
from sys import argv
import re
from SourceCodeRunInfo import get_run_info_by_language_name, ISourceCodeRunInfo
from verdict import Verdict
from exceptions import *

TEST_DIRECTORY = 'tests'
TEMPORARY_FILENAME = 'Program'
SUFFIX_ANSWER_FILENAME = '.a'
STUDENT_USER = 'student'


class TaskCodeRunner:
    def __init__(self, source_code_run_info: ISourceCodeRunInfo):
        self.__source_code_run_info = source_code_run_info
        self.__runnable_file = None

    def build(self, code_filename: str, result_filename: str):
        command = self.__source_code_run_info.format_build_command(code_filename, result_filename).split()
        process = Popen(command, stdout=DEVNULL, stderr=PIPE)
        _, err = process.communicate()
        if process.returncode != 0:
            raise CompilationException(err.decode())


    def run(self, test_file: str):
        command = self.__source_code_run_info.format_run_command(self.__runnable_file)
        process = Popen(['su', STUDENT_USER, '-c', f"{command}"], stdin=open(test_file, 'rb'), stdout=open(f'{test_file}.o', 'wb'), stderr=PIPE)
        try:
            err = process.communicate(timeout=time_limit)
        except TimeoutExpired:
            raise TimeLimitException()

        if process.returncode != 0:
            if b'PermissionError' in err[1]:
                raise SecurityException()
            raise RuntimeException()

    def run_test(self, code_filename: str, test_file: str):
        if self.__runnable_file is None:
            if self.__source_code_run_info.need_build():
                if self.__source_code_run_info.file_extension() == '.java':
                    self.__runnable_file = f'{code_filename[:-5]}'
                else:
                    self.__runnable_file = TEMPORARY_FILENAME
                self.build(code_filename, self.__runnable_file)
            else:
                self.__runnable_file = code_filename

        self.run(test_file)

def is_test_file(filename):
    return re.match('^\d+$', filename) is not None


def check(source_code_run_info, code_filename):
    runner = TaskCodeRunner(source_code_run_info)
    for number_test, test_filename in enumerate(sorted(filter(is_test_file, listdir(TEST_DIRECTORY))), 1):
        try:
            runner.run_test(code_filename, path_join(TEST_DIRECTORY, test_filename))
            process = Popen([path_join('.', 'check'),
                             path_join(TEST_DIRECTORY, test_filename),
                             f'{path_join(TEST_DIRECTORY, test_filename)}.o',
                             path_join(TEST_DIRECTORY, f'{test_filename}{SUFFIX_ANSWER_FILENAME}')],
                            stdout=DEVNULL, stderr=PIPE)
            _, err = process.communicate()
            if not err.startswith(b'ok'):
                raise WrongAnswerException()
        except CompilationException as e:
            return {
                'Verdict': Verdict.CompilationError.name,
                'CompilationOutput': e.message()
            }
        except RuntimeException:
            return {
                'Verdict': Verdict.RuntimeError.name,
                'Output': f'Тест №{number_test}'
            }
        except TimeLimitException:
            return {
                'Verdict': Verdict.TimeLimit.name,
                'Output': f'Тест №{number_test}'
            }
        except WrongAnswerException:
            return {
                'Verdict': Verdict.WrongAnswer.name,
                'Output': f'Тест №{number_test}'
            }
        except SecurityException:
            return {
                'Verdict': Verdict.SecurityException.name
            }
    return {
        'Verdict': Verdict.Ok.name,
    }

def remove_region_on_solution(filename):
    with open(filename, 'r') as f:
        data = f.read()

    data = re.sub('(#|\/\/)\s*(pragma\s+)?region\s+Task', '', data)
    data = re.sub('(#|\/\/)\s*(pragma\s+)?endregion\s+Task', '', data)

    with open(filename, 'w') as f:
        f.write(data)

def get_code_filename(old_filename, source_code_run_info):
    if source_code_run_info.file_extension() == '.java' :
        with open(old_filename, 'r') as f:
            data = f.read()
        return re.search('class\s*([\w\s]+?)\s*{', data).group(1) + '.java'
    return old_filename.replace('.any', source_code_run_info.file_extension())


language = argv[1].lower()
time_limit = float(argv[2].replace(',', '.'))
solution_filename = 'Program.any'
rename(path_join('solutions', argv[3]), solution_filename)
shutil.rmtree('solutions')
TaskCodeRunner(get_run_info_by_language_name('cpp')).build('check.cpp', 'check')  # Скомпилировали чеккер
run_info = get_run_info_by_language_name(language)
solution_filename_by_language = get_code_filename(solution_filename, run_info)
rename(solution_filename, solution_filename_by_language)
remove_region_on_solution(solution_filename_by_language)
result = check(run_info, solution_filename_by_language)
print(result)
