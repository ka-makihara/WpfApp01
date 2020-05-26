#-*- coding:utf-8 -*-
#
#
import sys

#clr.AddReference("IronPython")
#clr.AddReference("IronPython.Modules")
#
# os.getcwd() の実行において、カレントフォルダに日本語がはいっていると、エラーとなった
# ascii コードの問題のようだが・・・確か何か解決策があったような・・・

import os

with open('aa.txt','w') as f:
	f.write('test.py execute\n')
	f.write(pyStr+'\n')
	f.write(str(glo)+'\n')
	f.write(str(f_data)+'\n')
	f.write(os.getcwd()+'\n')
	for s in sys.path:
		f.write(s+'\n')
#	pyresult = "return"
	pyresult = ['return','abc','def']

	ftpToolExe.ConsoleOut('message')

def test_func(arg):
	with open('bb.txt','w') as f:
		f.write( str(arg) + '\n')
