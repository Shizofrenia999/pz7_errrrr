using System;

namespace FinancialCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== ФИНАНСОВЫЙ КАЛЬКУЛЯТОР ===");
                Console.WriteLine("1. Расчет кредита");
                Console.WriteLine("2. Конвертер валют");
                Console.WriteLine("3. Калькулятор вкладов");
                Console.WriteLine("4. Выход");
                Console.Write("Выберите опцию: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CalculateLoan();
                        break;
                    case "2":
                        ConvertCurrency();
                        break;
                    case "3":
                        CalculateDeposit();
                        break;
                    case "4":
                        Console.WriteLine("До свидания!");
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Нажмите любую клавишу для продолжения...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        // Модуль 1: Расчет кредита
        static void CalculateLoan()
        {
            Console.Clear();
            Console.WriteLine("=== РАСЧЕТ КРЕДИТА ===");

            try
            {
                Console.Write("Сумма кредита (руб): ");
                double loanAmount = GetPositiveNumber();

                Console.Write("Срок кредита (месяцев): ");
                int loanTerm = GetPositiveInteger();

                Console.Write("Процентная ставка (% годовых): ");
                double interestRate = GetNonNegativeNumber();

                if (interestRate == 0)
                {
                    Console.WriteLine("Внимание: Процентная ставка 0% - кредит без процентов!");
                }

                // Расчет ежемесячной процентной ставки
                double monthlyRate = interestRate / 12 / 100;

                // Расчет аннуитетного платежа
                double monthlyPayment = loanAmount * (monthlyRate * Math.Pow(1 + monthlyRate, loanTerm)) /
                            (Math.Pow(1 + monthlyRate, loanTerm) - 1);

                double totalPayment = monthlyPayment * loanTerm;
                double overpayment = totalPayment - loanAmount;

                Console.WriteLine("\n=== РЕЗУЛЬТАТЫ РАСЧЕТА ===");
                Console.WriteLine($"Ежемесячный платеж: {monthlyPayment:F2} руб");
                Console.WriteLine($"Общая сумма выплат: {totalPayment:F2} руб");
                Console.WriteLine($"Переплата по кредиту: {overpayment:F2} руб");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        // Модуль 2: Конвертер валют
        static void ConvertCurrency()
        {
            Console.Clear();
            Console.WriteLine("=== КОНВЕРТЕР ВАЛЮТ ===");
            Console.WriteLine("Доступные валюты: RUB, USD, EUR");

            try
            {
                Console.Write("Исходная валюта: ");
                string fromCurrency = GetValidCurrency();

                Console.Write("Целевая валюта: ");
                string toCurrency = GetValidCurrency();

                Console.Write("Сумма для конвертации: ");
                double amount = GetNonNegativeNumber();

                if (amount == 0)
                {
                    Console.WriteLine("Сумма для конвертации равна 0.");
                    Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                    return;
                }

                double result = ConvertCurrencyAmount(fromCurrency, toCurrency, amount);

                Console.WriteLine($"\n=== РЕЗУЛЬТАТ КОНВЕРТАЦИИ ===");
                Console.WriteLine($"{amount:F2} {fromCurrency} = {result:F2} {toCurrency}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        static double ConvertCurrencyAmount(string from, string to, double amount)
        {
            // Фиксированные курсы
            double usdToRub = 90.0;
            double eurToRub = 98.5;
            double eurToUsd = 1.09;

            // Если валюты одинаковые
            if (from == to)
                return amount;
            // Конвертация через RUB как базовую валюту
            double amountInRub = 0;

            // Конвертируем исходную валюту в RUB
            switch (from)
            {
                case "RUB":
                    amountInRub = amount;
                    break;
                case "USD":
                    amountInRub = amount * usdToRub;
                    break;
                case "EUR":
                    amountInRub = amount * eurToRub;
                    break;
                default:
                    throw new ArgumentException($"Неизвестная валюта: {from}");
            }

            // Конвертируем из RUB в целевую валюту
            switch (to)
            {
                case "RUB":
                    return amountInRub;
                case "USD":
                    return amountInRub / usdToRub;
                case "EUR":
                    return amountInRub / eurToRub;
                default:
                    throw new ArgumentException($"Неизвестная валюта: {to}");
            }
        }

        // Модуль 3: Калькулятор вкладов
        static void CalculateDeposit()
        {
            Console.Clear();
            Console.WriteLine("=== КАЛЬКУЛЯТОР ВКЛАДОВ ===");

            try
            {
                Console.Write("Сумма вклада (руб): ");
                double depositAmount = GetPositiveNumber();

                Console.Write("Срок вклада (месяцев): ");
                int depositTerm = GetPositiveInteger();

                Console.Write("Процентная ставка (% годовых): ");
                double interestRate = GetNonNegativeNumber();

                if (interestRate == 0)
                {
                    Console.WriteLine("Внимание: Процентная ставка 0% - вклад без процентов!");
                }

                Console.Write("Тип вклада (1 - с капитализацией, 2 - без капитализации): ");
                string depositType = GetValidDepositType();

                double income = 0;
                double totalAmount = 0;

                if (depositType == "1")
                {
                    // С капитализацией
                    double monthlyRate = interestRate / 12 / 100;
                    totalAmount = depositAmount * Math.Pow(1 + monthlyRate, depositTerm);
                    income = totalAmount - depositAmount;
                }
                else if (depositType == "2")
                {
                    // Без капитализации
                    income = depositAmount * interestRate * depositTerm / 12 / 100;
                    totalAmount = depositAmount + income;
                }

                Console.WriteLine("\n=== РЕЗУЛЬТАТЫ РАСЧЕТА ===");
                Console.WriteLine($"Доход по вкладу: {income} руб");
                Console.WriteLine($"Итоговая сумма: {totalAmount} руб");
                Console.WriteLine($"Тип вклада: {(depositType == "1" ? "с капитализацией" : "без капитализации")}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        // Вспомогательные методы для валидации

        static double GetPositiveNumber()
        {
            while (true)
            {
                if (double.TryParse(Console.ReadLine(), out double number))
                {
                    if (number > 0)
                        return number;
                    else
                        Console.Write("Ошибка: Число должно быть положительным. Попробуйте снова: ");
                }
                else
                {
                    Console.Write("Ошибка: Введите корректное число: ");
                }
            }
        }

        static double GetNonNegativeNumber()
        {
            while (true)
            {
                if (double.TryParse(Console.ReadLine(), out double number))
                {
                    if (number >= 0)
                        return number;
                    else
                        Console.Write("Ошибка: Число не может быть отрицательным. Попробуйте снова: ");
                }
                else
                {
                    Console.Write("Ошибка: Введите корректное число: ");
                }
            }
        }

        static int GetPositiveInteger()
        {
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int number))
                {
                    if (number > 0)
                        return number;
                    else
                        Console.Write("Ошибка: Число должно быть положительным. Попробуйте снова: ");
                }
                else
                {
                    Console.Write("Ошибка: Введите целое число: ");
                }
            }
        }

        static string GetValidCurrency()
        {
            string[] validCurrencies = { "RUB", "USD", "EUR" };
            while (true)
            {
                string input = Console.ReadLine().ToUpper().Trim();

                if (Array.Exists(validCurrencies, currency => currency == input))
                    return input;
                else
                    Console.Write("Ошибка: Доступные валюты: RUB, USD, EUR. Попробуйте снова: ");
            }
        }

        static string GetValidDepositType()
        {
            while (true)
            {
                string input = Console.ReadLine().Trim();

                if (input == "1" || input == "2")
                    return input;
                else
                    Console.Write("Ошибка: Введите 1 (с капитализацией) или 2 (без капитализации): ");
            }
        }
    }
}