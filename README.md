# IntDorSys

IntDorSys — система управления прачечной с двумя каналами работы:
- веб-интерфейс администратора;
- Telegram-бот для дублирующих административных сценариев.

Проект позволяет управлять пользователями, ролями и событиями использования, а также работать с отчетностью.

## Структура репозитория

В корне находятся два независимых приложения:

- `IntDorSys/` — backend на .NET (API, доменная логика, БД, интеграция с Telegram).
- `ui/` — admin UI на Angular (работает поверх backend API).

Ключевые точки входа:
- backend: `IntDorSys/IntDorSys.Web.Api/Program.cs`;
- frontend: `ui/src/main.ts`.

## Технологии

- Backend: .NET, ASP.NET Core Web API, Entity Framework, Blazor.
- Frontend: Angular.
- Интеграции: Telegram Bot API.

## Требования

- .NET SDK (совместимый с решением `IntDorSys.sln`).
- Node.js и npm (для `ui/`).
- Доступ к настроенной базе данных.

## Быстрый старт

### 1) Запуск backend API

> Важно: backend-команды запускайте из каталога `IntDorSys/`, чтобы корректно использовался `NuGet.Config`.

```bash
cd IntDorSys
dotnet restore IntDorSys.sln --configfile NuGet.Config
dotnet build IntDorSys.sln
dotnet run --project IntDorSys.Web.Api
```

По умолчанию локальный API и Blazor приложение работает на `http://localhost:5050`.

Альтернативно можно использовать скрипт:

```bash
cd IntDorSys
web-run.bat
```

### 2) Запуск admin UI

```bash
cd ui
npm install
npm start
```

Dev-сервер UI поднимается на `http://localhost:4201`.

## Конфигурация

- Основная конфигурация backend хранится в `IntDorSys/IntDorSys.Web.Api/appsettings.json`.
- Базовый URL API для UI задается в `ui/src/environments/environment.ts` и по умолчанию указывает на `http://localhost:5050/`.

## База данных и миграции

- Приложение может применять миграции автоматически при старте API (если включено `AutomaticMigrationsEnabled`).
- Перед запуском проверьте строку подключения и доступность базы данных.

