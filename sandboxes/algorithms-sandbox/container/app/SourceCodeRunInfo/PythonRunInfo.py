from SourceCodeRunInfo.ISourceCodeRunInfo import ISourceCodeRunInfo


class PythonRunInfo(ISourceCodeRunInfo):
    def format_build_command(self, code_filename: str, result_filename: str) -> list:
        return ''

    def need_build(self) -> bool:
        return False

    def format_run_command(self, filename: str) -> list:
        return f'python3 {filename}'
