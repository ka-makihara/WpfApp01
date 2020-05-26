#-*- coding:utf-8 -*-


import os
import sys

sys.path.append('C:\\Users\\ka.makihara\\source\\repos\\WpfApp1\\WpfApp1\\bin\\Debug')

import ftpUtil

def step_1(host):

	ftp = ftpUtil.connectFTP(host)

	#ftp.rename('Fuji','Fuji_app')
	#ftp.mkd('Fuji')
	ftpUtil.upload(ftp,'C:\\Users\\ka.makihara\\Desktop\\update_test\\Fuji','\\Fuji_tmp')
	
	ftp.quit()

if __name__ == '__main__':
	step_1('localhost')
