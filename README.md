# GeometryAndExchangeRate.Service

Простой Rest API веб сервис определения курса иностранной валюты к рублю РФ на дату.

## Требования

 - .NET 6.0 SDK (Linux, macOS or Windows)
 - .NET Runtime 6.0 (Linux, macOS, or Windows)
 - ASP .NET Core Runtime 6.0 (Linux, macOS, or Windows)

## Запуск
 1. Выполнить в консоли следующую команду:
    ```
    dotnet run --project ./src/Service/GeometryAndExchangeRate.Service.csproj
    ```
 2. Сервер выведет в консоль информацию. Скопировать и открыть в браузере один из указанных адресов.

## Пример использования

```
curl -X 'GET' \
  'https://localhost:7110/exchangerates/x0.5y0.5' \
  -H 'accept: text/plain'
```

Параметры `x` и `y` задают точку, по которой рассчитывается дата согласно следующему алгоритму: 
  - Первый квадрант: сегодня
  - Второй квадрант: вчера
  - Третий квадрант: позавчера
  - Четвертый квадрант: завтра
Точка не должна выходить за пределы окружности с центром в начале координат и радиусом, заданным в настройках приложения.

## Настройки приложения

Все глобальные переменные задаются в файле appsettings.json:
 - Logging:File:Path - задает имя файла, в который приложение записывает информацию о вызовах методов контроллеров.
 - DwsClient:DwsApiAddress - задает адрес API ЦБ РФ.
 - DwsClient:CurrencyCode - задает ISO код иностранной валюты (например USD).
 - QuadrantBasedPointToDateConverter:CircleRadius - задает радиус окружности.