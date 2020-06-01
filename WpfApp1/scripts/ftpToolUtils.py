#-*- coding:utf-8 -*-

#
# FTP_Tool Utilities
#
#import clr

#clr.AddReference("IronPython")
#clr.AddReference("IronPython.Modules")

import os
import sys
import glob
import ConfigParser

from importlib import import_module

import  ftpUtil

result = None

def getScriptFiles(path):
	global result

	#with open('pp.txt','w') as f:
	#    f.write(path+'\n')
	result = glob.glob(path+'\\*.py')

def connect(host, iniFile):
	global result

	ftp = ftpUtil.connectFTP(host)

	ftpUtil.get_file(ftp,'/Fuji/System3/Program/Peripheral/UpdateCommon.inf',iniFile)

	result = get_iniFile_sections(iniFile)

	ftp.close()

def get_iniFile_sections(path):
	global result
	
	cur = os.getcwd()
	ini = ConfigParser.ConfigParser()
	ini.read(path)

	result = ini.sections()
	return result

def getFuncList(scriptFile):
	global result

	scriptModule = os.path.splitext(os.path.basename(scriptFile))[0]
	module = import_module(scriptModule)

	result = [x for x in dir(module) if 'update_' in x]

def main():
	rr = connect('localhost','common.ini')

if __name__ == '__main__':
	main()