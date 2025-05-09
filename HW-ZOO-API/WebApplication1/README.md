# HSE Zoo API

## Реализованный функционал

1. **Управление животными**
   - Создание, просмотр, удаление животных, изменение статуса здоровья, кормление через `AnimalController`, `Animal`, `InMemoryAnimalRepository`

2. **Управление вольерами**
   - Создание, просмотр, удаление вольеров, очистка через `EnclosureController`, `Enclosure`, `InMemoryEnclosureRepository`

3. **Перемещение животных между вольерами**
   - Сервис `AnimalTransferService` для логики перемещения животных
   - API для перемещения через `AnimalTransferController`
   - Проверка совместимости типов животных с типами вольеров, добавление, удаление, проверка доступности вольеров через `Enclosure`
   - Генерация события `AnimalMovedEvent` при успешном перемещении

4. **Управление расписанием кормления**
   - Создание, просмотр и удаление расписания кормления через `FeedingScheduleController`
   - Сервис `FeedingOrganizationService` для организации процесса кормления
   - Маркировка выполненных кормлений, обновление расписания в `FeedingSchedule`
   - Уведомление о времени кормления через событие `FeedingTimeEvent`

5. **Статистика зоопарка**
   - Сбор и отображение статистики с помощью `ZooStatisticsController` и `ZooStatisticsService`
   - Подсчет общего количества животных, свободных вольеров
   - Группировка животных по видам и статусу здоровья
   - Отслеживание предстоящих и выполненных кормлений

## Применение концепций Domain-Driven Design

1. **Богатая доменная модель**
   - Класс `Animal` инкапсулирует логику по управлению кормлением, лечением, болезнями, заходу и выхода из вольера животного
   - Класс `Enclosure` содержит бизнес-правила по добавлению, удалению животных из вольера, очисткой, проверкой на доступность, совместимость с животным
   - Класс `FeedingSchedule` управляет статусом выполнения и изменением расписания

2. **Value Objects**
   - `AnimalType` - типы животных: хищники, травоядные, птицы, водные
   - `EnclosureType` - типы вольеров для хищников, травоядных, птиц, водных
   - `FoodType` - типы пищи для животных: мясо, фрукты, семена
   - `Gender` - пол животных: самка, самец
   - `HealthStatus` - статус здоровья животных: здоров, болен
   - `Name` - имя животных: не пустая строка из не более чем 30 символов
   - `NonNegativeInt` - целочисленное неотрицательное число

3. **Доменные события**
   - `AnimalMovedEvent` - событие при перемещении животного между вольерами
   - `FeedingTimeEvent` - событие при наступлении времени кормления
   - События публикуются через интерфейс `IEventPublisher`

4. **Инкапсуляция бизнес-правил**
   - Закрытые сеттеры свойств в сущностях для обеспечения инвариантов
   - Валидация при создании сущностей в value objects
   - Использование методов доменных объектов для изменения их состояния

## Применение принципов Clean Architecture

1. **Разделение на слои**
   - **Domain**: Содержит сущности `Animal`, `Enclosure`, `FeedingSchedule`, value objects и доменные события
   - **Application**: Содержит сервисы `AnimalTransferService`, `FeedingOrganizationService`, `ZooStatisticsService` и интерфейсы репозиториев
   - **Infrastructure**: Реализация репозиториев, событийной системы
   - **Presentation**: API контроллеры и DTO-модели

2. **Зависимости направлены внутрь**
   - Доменный слой не зависит ни от какого другого слоя
   - Слой приложений зависит только от доменного слоя
   - Инфраструктурный слой зависит от слоев приложений и домена
   - Слой представления зависит от всех остальных слоев

3. **Инъекция зависимостей**
   - Зависимости между слоями реализованы через интерфейсы
   - Репозитории определены в Application слое, реализованы в Infrastructure

4. **Изолированная бизнес-логика**
   - Ключевая бизнес-логика сосредоточена в доменных сущностях и сервисах приложения
   - Контроллеры только делегируют вызовы сервисам и преобразуют данные в DTO
   - Репозитории отвечают только за хранение и извлечение данных