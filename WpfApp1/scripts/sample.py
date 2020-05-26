#-*- coding:utf-8 -*-
#
import ConfigParser

def sample_func(path):
	with open('cc.txt','w') as f:
		f.write('sample.txt'+'\n')
		f.write(str(path)+'\n')


result = None

def get_iniFile_sections(path):
	global result
	
	ini = ConfigParser.ConfigParser()
	ini.read(path)

	sec = ini.sections()
	with open('cc.txt','w') as f:
		for s in sec:
			f.write(s+'\n')

	result = sec
