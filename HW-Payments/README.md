# Система создания заказов и обработки платежей (HW-Payments)

## Реализованный функционал

1. **Обработка заказов**
   - Создание и управление заказами через `OrdersController`
   - Хранение информации о заказах в `Order`
   - Отправка запросов на оплату через `RabbitMqMessageBroker`
   - Обработка статуса платежей через `PaymentStatusConsumerService`

2. **Обработка платежей**
   - Управление счетами через `AccountsController`
   - Обработка платежных транзакций через `PaymentsController`
   - Хранение информации о счетах в `Account`
   - Хранение информации о транзакциях в `Transaction`

3. **API Gateway**
   - Маршрутизация запросов между микросервисами
   - Единая точка входа в приложение

## Применение микросервисной архитектуры

1. **Разделение на независимые сервисы**
   - **OrdersService**: отвечает за создание и управление заказами
   - **PaymentsService**: отвечает за обработку платежей и управление счетами
   - **APIGateway**: обеспечивает маршрутизацию запросов между сервисами

2. **Асинхронное взаимодействие через сообщения**
   - Использование RabbitMQ для обмена сообщениями между сервисами
   - Паттерн Outbox для надежной доставки сообщений
   - Паттерн Inbox для идемпотентной обработки сообщений
   - Итого реализована семантика Exactly once.

3. **Изоляция данных**
   - Отдельная база данных для каждого сервиса
   - Повышенная отказоустойчивость: при отказе одной БД другие сервисы продолжают работу
   - Взаимодействие между сервисами только через API или сообщения

## Структура проекта

1. **OrdersService**
   - `Controllers/OrdersController.cs`: API для управления заказами
   - `Data/OrderDbContext.cs`: контекст базы данных для хранения заказов
   - `DTOs/CreateOrderRequest.cs`: структура запроса для создания заказа  
   - `Models/Order.cs`: модель заказа
   - `Repositories/OrderRepository.cs`: репозиторий для CRUD операций над заказами
   - `Services/OrderService.cs`: сервис для обработки бизнес-логики заказов
   - `Services/OutboxProcessorService.cs`: сервис для обработки исходящих сообщений
   - `Services/PaymentStatusConsumerService.cs`: сервис для обработки статуса платежей

2. **PaymentsService**
   - `Controllers/AccountsController.cs`: API для управления счетами
   - `Controllers/PaymentsController.cs`: API для обработки платежей
   - `Data/PaymentDbContext.cs`: контекст базы данных для хранения платежей и аккаунтов
   - `Models/Account.cs`: модель счета
   - `Models/Transaction.cs`: модель транзакции
   - `Models/InboxMessage.cs`: модель входящего сообщения
   - `Repositories/AccountRepository.cs`: репозиторий для CRUD операций над аккаунтами
   - `Repositories/InboxRepository.cs`: репозиторий для CRUD операций над входящими сообщениями
   - `Repositories/TransactionRepository.cs`: репозиторий для CRUD операций над платежами
   - `Services/AccountService.cs`: сервис для управления счетами
   - `Services/PaymentService.cs`: сервис для обработки платежей
   - `Services/OutboxProcessorService.cs`: сервис для обработки исходящих сообщений
   - `Services/PaymentStatusConsumerService.cs`: сервис для обработки запросов с платежами

4. **Shared**
   - `Messaging/Infrastructure/MessageBroker/RabbitMqMessageBroker`: общая логика работы с очередью сообщений между микросервисами
   - `Messaging/Infrastructure/Outbox/BaseOutboxProcessorService`: общий сервис для обработки сообщений с помощью паттерна Outbox
   - `Messaging/Infrastructure/Outbox/OutboxMessage`: структура исходящего сообщения для реализации паттерна Outbox
   - `Messaging/Messages/PaymentRequestMessage`: сообщение о запросе на платёж из OrderService в PaymentsService
   - `Messaging/Messages/PaymentStatusMessage`: сообщение о статусе платёжа из PaymentsService в OrderService 

3. **APIGateway**
   - `Program.cs`: конфигурация API-шлюза и маршрутизации запросов
   - `appsettings.json`: настройки маршрутизации и сервисов

## Запуск проекта

1. **Запуск с помощью Docker Compose**
   ```
   docker compose up --build
   ```

2. **Доступ к API**
   - API Gateway: `http://localhost:7001/api/*`
   - Orders Service: `http://localhost:7002/api/orders`
   - Payments Service:
     - `http://localhost:7003/api/payments`
     - `http://localhost:7003/api/accounts`