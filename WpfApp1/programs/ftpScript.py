#
#
#
import ftpUtil

def step_1(host):
	'''
		周辺機器をアップデートする
	'''
	ftp = ftpUtil.connectFTP(host)

	ftp.rename('Fuji','Fuji_app')
	ftp.mkd('Fuji')
	ftp.Upload("sjdksl.om",'Fuji')
	ftpUtil.ftp_put(ftp,'Fuji')
	ftp.quit()
	Message('再起動してください')
	Message('再起動が完了したら周辺機器のバージョンアップを実行してください')

def step_2(host):
	'''
		黒画面用の eufaula, hdd_bootimg
		safemodeがアップデートされる
	'''
	ftp = ftpUtil.connectFTP(host)

	ftp.rename('eufaula','eufaula_app')
	ftp.rename('hdd_bootimg','hdd_bootimg_app')
	ftpUtil.ftp_put(ftp,'eufaula')
	ftpUtil.ftp_put(ftp,'hdd_bootimg')
	ftp.quit()
	Message('再起動してください')
	'セーフモードのバージョンを確認する'
	
def step_3(host):
	'''
		黒画面用の eufaula, hdd_bootimg
		safemodeがアップデートされる
	'''
	ftp = ftpUtils.connectFTP(host)

	ftp.rename('eufaula','eufaula_app')
	ftp.rename('hdd_bootimg','hdd_bootimg_app')
	ftpUtil.ftp_put(ftp,'eufaula')
	ftpUtil.ftp_put(ftp,'hdd_bootimg')
	ftp.quit()
	Message('再起動してください')
	'セーフモードのバージョンを確認する'


def step_3(host):
	'''
		セーフモード以外を元に戻す
	'''
	ftp = ftpUtil.connectFTP(host)
	ftp.rename('eufaula_app','eufaula')
	ftp.rename('hdd_bootimg_app','hdd_bootimg')
	ftp.rename('Fuji_app','Fuji')
	ftp.quit()
	Message('再起動してください')
	# 固定基板、X軸のバージョンアップ画面
	# 実行後、MCで再起動を促すダイアログ
	# 再起動で通常画面(ヘッドが生焼け)


def all_steps(host):
	step_1(host)
	step_2(host)
	step_3(host)
