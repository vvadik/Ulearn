import re

def test_regex(ans, regs):
    for x in regs:
        if re.match(r'^' + x + r'$', ans):
            return True
    return False
  
def test_regex_1(expected, answer):
    return test_regex(answer, [r'[dD]irectory', r'[Ss]ystem.[Ii][Oo].[Dd]irectory'])

def test_regex_2(expected, answer):
    return test_regex(answer, [r'9'])
