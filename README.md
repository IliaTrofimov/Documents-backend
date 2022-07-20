# Documents-backend
Система управления документацией информационных систем (серверная часть).

Приложение позволяет создавать шаблоны для документов (набор полей с ограничениями на заполняемые данные), создавать документы и печатать их в PDF с помощью
добавленных html печатных форм. Аутентификации нет т.к. предполагалось использовать встроенную аутентификацию компании (но информации по ней я не получил).

Клиентская часть приложения [здесь](https://github.com/IliaTrofimov/Documents-frontend).

## Структура проекта
* **Documents-MailSender** - консольный проект для запуска рассылки уведомлений;
	* *Program.cs* - точка входа в приложение;
* **App_start/**
	* *FilterConfig.cs* - перехват ошибок;
	* *WebAapiConfig.cs* - настройки WebApi, маршрутизация;
* **Controllers/** - контроллеры;
	* *PrintingController.cs* - контроллер для работы с печатными формами и создания pdf документов;
	* *MailingController.cs* - служебный контроллер для рассылки эл. почты;
	* *DefaultController.cs* - служебный контроллер, содержт эндпоинты для проверки работы сервера. Нужно удалить потом;
	* Остальные контроллеры работают со своими сущностями, тривиально;
* **Migration/** - здесь должны быть миграции для MS SQL;
	* *Configuration.cs* - настройки миграций.
* **Models/** - все сущности;
	* *DataContext.cs* - класс контекста данных Entity Framework;
	* **Administrative/** *UserPermissions.cs* - модель прав доступа пользователей;
	* **DTO/** - Data Transfer Objects, скорее всего удалю это, так как вместо них можно использовать ленивую загрузку данных в Entity Framework;
	* **Entities/** - модели для БД;
	* **Post/** - модели для body некоторых POST и PUT запросов;
* **Services/** - вспомогательные сервисы;
	* **Mailing/** - почта: отправление уведомлений о подписании документов и истечения срока действия документов;
		* *MailingClient.cs* - класс для отправки писем;
	* **Printing/** - 
		* *DocumentPrinter.cs* - класс для создания печатных форм и преобразования их в pdf;
* **Utility/** - всякое разное и нужное;
	* *ControllerExtentions.cs* - методы-расширения для контроллеров;
* **Properties/** 
	* *Settings.settings* - настройки приложения (могут автоматически дублироваться в web.config);
	* *ConnectionStrings.config* - строки подключения к БД, импортируется в web.config (см далее).

## Маршрутизация
* **api/** - корень
	* **default/** - тестирование
		* list - метод GET (простая проверка работы сервера)
		* test - метод GET (обращение ко всем таблицам в СУБД, проверка на наличие ошибок)
		* user - метод GET (возвращает текущий IPrincipal запроса)
	* **printing/**
		* add-template/{id} - метод GET (загружает html печатную форму для шаблона с указанным id, форма передаётся как multipart/form-data);
		* check-existance/{id} - метод GET (проверяет наличие в БД печатной форму для шаблона с указанным id);
		* get-template/{id}?download=true|flase - метод GET (отправляет пользователю html печатную форму для шаблона с указанным id, 
		если download = true, то выгрузит html как файл);
		* document-html/{id} - метод GET (отправляет пользователю заполненную форму (в виде html) для документа с указанным id);
		* document-pdf/{id} - метод GET (отправляет пользователю заполненную форму (в виде pdf) для документа с указанным id);
	* **mailing/** 
		* expiration-alert - метод GET (начинает рассылку уведомлений об истечении срока действия документов)
	* **documents/** 
		* count - (есть фильтрация), метод GET
		* list - (есть фильтрация), метод GET
		* post - метод POST, в body передаётся `{TemplateId: int, PreviousId: int, Name: string}`
		* {id}/get - метод GET
		* {id}/put - метод PUT, в body передаётся объект класса `Models.Entities.Document` (поля документа не изменяются)
		* {id}/put-field - метод PUT, в body передаётся объект класса `Models.Entities.DocumentDataItem`, {id} - Id документа
		* {id}/delete - метод DELETE
	* **templates/** 
		* count - (есть фильтрация), метод GET
		* list - (есть фильтрация), метод GET
		* post - метод POST, в body передаётся объект класса `Models.Entities.Template`
		* {id}/get - метод GET
		* {id}/put - метод PUT, в body передаётся объект класса `Models.Entities.Template` (поля шаблона не изменяются)
		* {id}/put-field - метод PUT, в body передаётся объект класса `Models.Entities.TemplateField`, {id} - Id шаблона
		* {id}/put-table - метод PUT, в body передаётся объект класса `Models.Entities.TemplateTable`, {id} - Id шаблона (ячейки таблицы не изменяются)
		* {id}/move-item - метод PUT, в body передаётся `{FirstItemId: int, SecondItemId: int}`, {id} - Id шаблона (меняет местами два элемента шаблона)
		* {id}/delete - метод DELETE
	* **signs/** 
		* count - (есть фильтрация), метод GET
		* list - (есть фильтрация), метод GET
		* post - метод POST, в body передаётся объект класса `Models.Entities.Sign`
		* {id}/notify - метод PUT (чтобы нельзя было просто так из браузера перейти)
		* {id}/get - метод GET
		* {id}/put - метод PUT, в body передаётся объект класса `Models.Entities.Sign`
		* {id}/delete - метод DELETE
	* **templatetypes/** 
		* count - (есть фильтрация), метод GET
		* list - (есть фильтрация), метод GET
		* post - метод POST, в body передаётся объект класса `Models.Entities.TemplateType`
		* {id}/get - метод GET
		* {id}/put - метод PUT, в body передаётся объект класса `Models.Entities.TemplateType`
		* {id}/delete - метод DELETE
	* **positions/** 
		* count - (есть фильтрация), метод GET
		* list - (есть фильтрация), метод GET
		* post - метод POST, в body передаётся `Models.Entities.Position`
		* {id}/get - метод GET
		* {id}/put - метод PUT, в body передаётся объект класса `Models.Entities.Position`
		* {id}/delete - метод DELETE
	* **users/** 
		* whoami - метод GET
		* count - (есть фильтрация), метод GET
		* list - (есть фильтрация), метод GET
		* post - метод POST, в body передаётся объект класса `Models.Entities.User`
		* {id}/get - метод GET
		* {id}/put - метод PUT, в body передаётся передаётся объект класса `Models.Entities.User`
		* {id}/delete - метод DELETE 

Все эндпоинты поддерживающие фильтрацию используют следующие query-параметры для пагинации: `{page: int, pageSize: int}`. Значение -1 для отключения фильтра. 
Некоторые контроллеры поддерживают дополнительные фильтры:
* documents: `{template: int, user: int, type: int}`;
* templates: `{user: int, type: int, showDepricated: bool}`;
* signs: `{documentId: int, userId: int, initiatorId: int, showOld: bool, showUnassigned: bool}`;
* users: `{position: int, permissions: int}`.

## Настройки
Файл **web.config**

**Строки подключения** необходимо добавлять в web.config следующим образом:
```xml
<configuration>
	...
	<connectionStrings configSource="Properties\\ConnectionStrings.config"></connectionStrings>
	...
</configuration>
```
Здесь `configSource` задаёт путь до файла со строками подключения, который должен иметь следующий вид:
```xml
<?xml version="1.0" encoding="utf-8"?>
<connectionStrings>
	<add connectionString="Server=WRUC003638\SQLEXPRESS;Database=documents;Integrated Security=SSPI;" 
		 name="DataContext" 
		 providerName="System.Data.SqlClient"
	/>
	... и т.д.
</connectionStrings>
```
Стандартные **настройки приложения** хранятся в Properties/Settings.settings, но всегда дублируются в web.config:
```xml
<configuration>
	...
	<applicationSettings>
		<Documents.Properties.Settings>
			<setting name="EmailPort" serializeAs="String">
				<value>465</value>
			</setting>
			... и т.д.
		</Documents.Properties.Settings>
	</applicationSettings>
</configuration>
```
Нет смысла их оттуда убирать, при сборке они вновь сгенерируются или, возможно, Visual Studio будет кидаться ошибками. 

Настройки можно менять в визуальном редакторе Visual Studio (Проект→Свойства→Настройки), или вручную изменяя файл Settings.settings.

Описание настроек:
* EmailPort - порт для SMTP клиента почты;
* EmailHost - хост для SMTP клиента почты;
* EmailPassword - пароль для SMTP клиента почты. Заполняйте, если используется SSL, если используется TLS - оставьте пустым;
* EmailLogin - логин для SMTP клиента почты. Заполняйте, если используется SSL, если используется TLS - оставьте пустым;
* EmailFrom - почтовый адрес, с которого будут приходить автоматически созданные письма;
* EmailFromName - имя отправителя созданных писем;
* BaseUrl -корневой url для создания ссылок в письмах.

 ## Миграции
 Перед первым запуском приложения необходимо включить миграции, для этого в консоле диспетчера пакетов введите:
```
Enable-migrations
``` 
Эта команда создаст все таблицы, которые указаны в DataContext и одну таблицу MigrationHistory в указанной в ConnectionString базе данных.

Чтобы синхронизировать БД с моделями C# после их изменения используйте команды:
```
Add-migration <имя миграции>
Update-database
```
Первая команда создаст файл миграции, вторая применит последний такой файл и обновит БД. Однако EntityFramework при генерации миграций может допускать ошибки, из-за которых обновить таблицы не получится. Это придётся исправлять, вручную изменяя файл миграции.

**Первый запуск**

Если при запуске миграции БД оказалось пустой (нет записей в таблице Users или в Positions нет администатора), 
то метод Configuration.Seed() создаст должность администратора и пользователя Admin с такой должностью с CWID '00000000',
его нужно будет изменить в MS SQL Server или в клиенте.


## CORS
В контроллерах могут быть аннотации вида 
```cs
[EnableCors(origins: "xxx", headers: "xxx", methods: "xxx", SupportsCredentials = true)]
```
если CORS не нужен, их можно удалить. 
При запуске локально без IIS без них нельзя. Также в Global.asax метод 
```cs
void Application_BeginRequest()
``` 
добавляет заголовки для запросов OPTIONS, его тоже можно удалить.
