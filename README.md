# trade-task

Сервис каждый час высчитывает разницу в цене между квартальными фьючерсами BTC_USDT_20250627 и BTC_USDT_20250926 на бирже Gate и отправляет результат в топик в кафке.

Это все можно поднять с помощью `docker-compose up -d`
 
Логи тут - [kibana](http://localhost:5601/app/discover#/?_a=(columns:!(),dataSource:(dataViewId:'047b9ce1c481e9105458e4238be7cbb304abc176b09c3b4d196d84686c42b5d0',type:dataView),filters:!(),interval:auto,query:(language:kuery,query:''),sort:!())&_g=(filters:!(),refreshInterval:(pause:!t,value:60000),time:(from:now-15m,to:now)))

Топики - [kafka-ui](http://localhost:8080/ui/clusters/local/all-topics?perPage=25)
