from SourceCodeRunInfo.ISourceCodeRunInfo import ISourceCodeRunInfo


class CppRunInfo(ISourceCodeRunInfo):
    def format_run_command(self, filename: str) -> list:
        return f'./{filename}'

    def need_build(self) -> bool:
        return True

    def format_build_command(self, code_filename: str, result_filename: str) -> list:
        return f'g++ -o {result_filename} {code_filename}'
