# Получим директорию с установленным пакетом из первого аргумента
installationDirectoryPath=$1

# Пропишем путь до исполняемого файла в файл задачи. Разрешены только полные пути.
sudo sed -i "s@%ExecStart%@/bin/dotnet $installationDirectoryPath/RunCheckerJob.dll@" deploy/runcheckerjob.service

# Скопируем файл задачи в папку дял созданных вручную задач
sudo cp deploy/runcheckerjob.service /etc/systemd/system

# Добавим задачу в автозапуск при старте системы
sudo systemctl enable runcheckerjob 

# Перезапустим демона задач, чтобы он перечитал файлы с диска
sudo systemctl daemon-reload

# Перезапустим или стартанем задачу
sudo systemctl restart runcheckerjob

# Выполним build.sh всех docker-контейнеров
for filename in sandboxes/*/container/build.sh
do
	# Путь до файла скрипта без имени файла
	container_dir=$(dirname $filename)
	echo Start build container with $filename
    # В скобках временно сменим рабочую директорию, после выхода из скобок она вернется на старую
	# Рабочая директория должна быть директорией с файлом build.sh, чтобы пути в нем были относительно неё
    (
		cd $container_dir
		sh build.sh
	)
done