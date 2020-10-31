from SourceCodeRunInfo.ISourceCodeRunInfo import ISourceCodeRunInfo
from SourceCodeRunInfo.CppRunInfo import CppRunInfo
from SourceCodeRunInfo.PythonRunInfo import PythonRunInfo


def get_run_info_by_language_name(language_name: str) -> ISourceCodeRunInfo:
    if language_name == 'cpp':
        return CppRunInfo()
    elif language_name == 'python':
        return PythonRunInfo()
