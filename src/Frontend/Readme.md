Привет.
Это краткий документ о том, как устроена сборка фронта.

Имеется 3 типа сборки : development, production, courseTool. У каждого типа свой конфиг webpack-а
Запуск Node-ом описан в файлах в папке scripts. В dev режиме запускается dev server для работы в браузере, подключается HMR

    "start": "node scripts/start.js", -- dev
    "build": "node scripts/build.js", -- prod
    "courseToolBuild": "node scripts/courseToolBuild.js", -- courseTool
    
Конфиги webpack описаны в папке config ( webpack.config.(dev||prod||courseTool) ). 
Имеется базовый конфиг webpack.config.base, в котором описаны alias-ы ( они нужны, чтобы вместо @skbkontur/react-ui писать ui)
Так же в этой папке имеются конфиги полифилов, сентри и т.д.
    
Имеются допольнительные возможности
    
    "analyze": "node scripts/build.js analyze", -- запуст production с bundleAnalyzer, для анализа сборки
    "test": "jest", -- запуск тестов
    "storybook": "start-storybook -p 9001 -c .storybook" -- запуск сторибука
    
Настройки бабеля находятся в ./babel.config.js


**Немного о том, как устроена взаимосвязь сервер - клиент**

Сервер, как и запросы, разделен на 2 части : Api и Web. 

Api отвечает за работу базы данных и тд (имеет адрес http:[::]:8000, api.ulearn.me)

Web запускает скрипт build, берет полученную (статичную) сборку, и отдает её на запросы в браузере.
Также отвечает за рендер cshtml страниц (https:[::]:44300, http://localhost:53834/, ulearn.me)

У фронта же есть dev server (http:[::]:3000)

Работа на боевом сервере: любой запрос либо страничка, либо api.
В первом случае домен не меняется, во втором отправляется запрос на api

DevServer: html формируется по webpack.config.dev, отдается по любому адресу
далее любой обычный запрос прокситься на web(44300). а api запрос отправляется по 8000 адресу

api адрес находится в window.config

На боевом это пропишет сервер ( когда будет возвращать html )
window.config={
  "api": {
    "endpoint": "https://api.ulearn.me/"
  }
}
на локале это прописано в src/config.js ( он берет адресс из settings.json)

