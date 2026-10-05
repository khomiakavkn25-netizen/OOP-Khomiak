# Звіт з аналізу інкапсуляції в Open-Source проєкті
## 1. Обраний проєкт
Назва: Serilog

Посилання на GitHub: https://github.com/serilog/serilog

## 2. Аналіз інкапсуляції
Клас: LogEventProperty

Посилання: LogEventProperty.cs

Опис: Представляє властивість події логування (пару «назва-значення»).

Поля: Використовуються приватні readonly поля.

Властивості: Read-only (тільки get через =>), що гарантує незмінність об'єкта (immutability).


Поля:

    readonly string _name;
    readonly LogEventPropertyValue _value;

Властивості:

    public string Name => _name;
    public LogEventPropertyValue Value => _value;
Клас: LogEvent

Посилання: LogEvent.cs

Опис: Основний клас події логування.

Поля та Властивості: Використовує автоматичні read-only властивості. Словник _properties закрито через IReadOnlyDictionary, що унеможливлює його модифікацію ззовні.


 Поле та захищена колекція:

    readonly Dictionary<string, LogEventPropertyValue> _properties;

    public IReadOnlyDictionary<string, LogEventPropertyValue> Properties => _properties;

Автоматичні read-only властивості:

    public DateTimeOffset Timestamp { get; }

    public LogEventLevel Level { get; }

Клас: ScalarValue

Посилання: ScalarValue.cs

Опис: Обгортка для простих (скалярних) значень.

Поля та Властивості: Приватне readonly поле та read-only властивість.

    readonly object? _value;

    public object? Value => _value;

## 3. Практики валідації у властивостях
Оскільки об'єкти є read-only і не мають set-аксесорів, валідація виконується в конструкторі при їх створенні:


    public LogEventProperty(string name, LogEventPropertyValue value)

    {

        if (name == null) throw new ArgumentNullException(nameof(name));

        if (value == null) throw new ArgumentNullException(nameof(value));

        if (!IsValidName(name)) throw new ArgumentException("Property name is not valid.", nameof(name));

        _name = name;
        _value = value;
    }   

## Висновки щодо валідації:
  Валідація в конструкторі з викиданням винятків (ArgumentNullException, ArgumentException) запобігає створенню об’єктів у некоректному стані. Це надійніше за set-аксесори, оскільки гарантує потокобезпечність і незмінність даних під час виконання програми.

## 4. Загальні висновки
У проєкті Serilog інкапсуляцію реалізовано через приватні readonly поля та публічні read-only властивості. Замість класичних сеттерів з перевірками використовується валідація даних у конструкторах. Такий підхід гарантує повну цілісність даних і робить код безпечним для використання.

Відповіді на контрольні запитання
За якими ознаками можна зробити висновок, що в класі дотримано інкапсуляції?
Поля є приватними (private), доступ надається лише через властивості або методи, а стан об'єкта захищено від довільної зміни ззовні.

Які підходи до валідації властивостей ви зустріли в реальному коді?
Валідацію в конструкторі через if з викиданням винятків (ArgumentNullException), а також перевірки у сеттерах set { ... }.

Чому для аналізу важливо наводити посилання на конкретні класи та фрагменти коду?
Це підтверджує практичну достовірність аналізу на основі реального open-source проєкту.

Які висновки щодо якості проєктування ви зробили після порівняння 2-3 класів?
Код проєкту дотримується єдиного стилю: перевага надається незмінним (read-only) структурам даних, що мінімізує помилки при роботі з об'єктами.