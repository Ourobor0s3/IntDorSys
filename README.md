# IntDorSys

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

Система управления прачечной: REST API + Telegram-бот + веб-интерфейс администратора.

## Архитектура

```
IntDorSys/
├── IntDorSys/              # Backend на .NET 8
│   ├── IntDorSys.Web.Api/  # ASP.NET Core Web API
│   ├── IntDorSys.Core/     # Доменная логика
│   ├── IntDorSys.DataAccess/ # EF Core, PostgreSQL
│   ├── IntDorSys.Security/ # JWT-аутентификация
│   └── ...
├── ui/                     # Admin UI на Angular 17
└── docker-compose.yml      # Развёртывание одной командой
```

## Технологии

| Слой | Технологии |
|---|---|
| **Backend** | .NET 8, ASP.NET Core, Entity Framework Core (Npgsql), Quartz Scheduler |
| **Database** | PostgreSQL 16 |
| **Frontend** | Angular 17, PrimeNG, ngx-bootstrap, ngx-translate, Chart.js |
| **Bot** | Telegram Bot API |
| **Deploy** | Docker, Docker Compose, GitHub Actions |

## Требования

- .NET 8 SDK
- Node.js 18+ и npm
- PostgreSQL 16 (или Docker)
- Telegram Bot Token (для бота)

## Быстрый старт

### Локально

```bash
# Backend (из папки IntDorSys/ — важно для NuGet.Config)
cd IntDorSys
dotnet restore IntDorSys.sln --configfile NuGet.Config
dotnet build IntDorSys.sln
dotnet run --project IntDorSys.Web.Api --urls "http://localhost:5050"

# Frontend
cd ui
npm install
npm start   # → http://localhost:4201
```

### Docker Compose

```bash
cp .env.example .env        # отредактируйте токены и пароли
docker compose up -d --build
```

Сервисы:
- **Backend**: http://localhost:5050
- **Frontend**: http://localhost:4201
- **PostgreSQL**: localhost:5433 (host) / 5432 (container)

## Конфигурация

Порядок загрузки: `appsettings.json` → `appsettings.local.json` (gitignored) → переменные окружения.

Ключевые настройки в `.env`:

| Переменная | Описание |
|---|---|
| `BUILD_TEST` | `true` — тестовый режим, `false` — боевой |
| `TELEGRAM_BATTLE_TOKEN` | Токен боевого бота |
| `TELEGRAM_TEST_TOKEN` | Токен тестового бота |
| `JWT_SECRET` | Ключ подписи JWT (мин. 32 символа) |
| `DB_HOST` / `DB_PORT` / `DB_NAME` | Параметры подключения к БД |
| `ADMIN_CHAT_ID` / `MANAGER_LAUNDRESS_ID` | Telegram ID администраторов |

Двухботовый режим: при `BUILD_TEST=true` используется `Telegram__Test__Token`, иначе `Telegram__Battle__Token`.

## API

Nginx проксирует запросы с префиксами `/(account|user|laund|users-info|analitic|token|health|file)` на бэкенд.

| Маршрут | Аутентификация | Описание |
|---|---|---|
| `POST /token` | Rate-limited (10/мин) | Вход → `{accessToken, role}` |
| `GET /health` | Нет | Health check |
| `GET /laund` | Auth | Список слотов |
| `POST /laund` | Admin | Создать слот |
| `POST /laund/range` | Admin | Массовое создание |
| `POST /laund/book` | Admin | Записать пользователя |
| `DELETE /laund/book` | Admin | Снять пользователя |
| `DELETE /laund` | Admin | Удалить пустой слот |
| `GET /laund/reports` | Admin | Отчёты |
| `GET /users-info` | Auth | Все пользователи |
| `PUT /users-info/change-status/{userId}` | Admin | Изменить статус |
| `GET /analitic/laund` | Admin | Данные для графиков |
| `GET /user` | Auth | Профиль текущего пользователя |
| `GET /file/{guid}` | Нет | Скачать файл |
| `POST /account` | Rate-limited | Регистрация |

## Telegram-бот

### Команды

| Команда | Роль | Действие |
|---|---|---|
| `/start` | Любой | Приветствие |
| `/rules` | Любой | Правила прачечной |
| `/send` | Студент | Отправить фотоотчёт менеджеру |
| `/dump` | Admin | Дамп БД |
| `/crlaund` | Менеджер | Создать слоты |
| `/rmlaund` | Менеджер | Удалить слоты |
| `/unuse` | Менеджер | Снять пользователя со слота |
| `/rmuse` | Менеджер | Удалить запись |
| `/setuser` | Менеджер | Записать пользователя на слот |

### Инлайн-меню

| Кнопка | Действие |
|---|---|
| ✍️ Записаться | Свободные даты |
| 📝 Мои записи | Записи пользователя |
| 📝 Все записи (менеджер) | Все записи |
| 😎 Доступ есть (admin) | Подтверждённые пользователи |
| 🤬 Доступа нет (admin) | Заблокированные пользователи |

При регистрации нового пользователя админ получает кнопки **Yes/No** для подтверждения. В списках пользователей нажатие на имя блокирует/разблокирует.

## CI/CD

Пуш в `main` → GitHub Actions:
1. `dotnet restore + build` — проверка сборки
2. SSH-деплой на сервер: `git pull && docker compose up -d --build`

## Лицензия

MIT — подробнее в [LICENSE](LICENSE).
