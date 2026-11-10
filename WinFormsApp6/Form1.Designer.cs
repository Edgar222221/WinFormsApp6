using System.Globalization;

namespace Calculator;

// Nem fókuszálható gomb: így a billentyűzet-események mindig az űrlaphoz érkeznek,
// és az Enter nem "nyomja meg újra" az utoljára kattintott gombot.
public class CalcButton : Button
{
    public CalcButton() => SetStyle(ControlStyles.Selectable, false);
}

public class CalculatorForm : Form
{
    private readonly Label expression = new(); // kis felirat: pl. "12 + 5 ="
    private readonly Label display = new();    // a nagy kijelző

    private decimal? accumulator;       // az eddigi részeredmény
    private char? pendingOp;            // függőben lévő művelet (+ − × ÷)
    private bool startNewEntry = true;  // a következő számjegy új számot kezd
    private bool justEvaluated;         // az utolsó gomb az "=" volt
    private bool error;

    private static readonly NumberFormatInfo Nfi = CultureInfo.CurrentCulture.NumberFormat;
    private static readonly string Dec = Nfi.NumberDecimalSeparator; // magyarul: ","
    private static readonly string Neg = Nfi.NegativeSign;

    public CalculatorForm()
    {
        Text = "Számológép";
        ClientSize = new Size(340, 480);
        MinimumSize = new Size(300, 440);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(243, 243, 243);
        Font = new Font("Segoe UI", 12f);
        KeyPreview = true;

        // --- Elrendezés: kifejezés / kijelző / gombrács ---
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(8)
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        expression.Dock = DockStyle.Fill;
        expression.AutoSize = false;
        expression.TextAlign = ContentAlignment.MiddleRight;
        expression.ForeColor = Color.DimGray;
        expression.Font = new Font("Segoe UI", 10f);

        display.Dock = DockStyle.Fill;
        display.AutoSize = false;
        display.TextAlign = ContentAlignment.MiddleRight;
        display.Font = new Font("Segoe UI Semibold", 24f);
        display.Text = "0";

        var grid = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 5
        };
        for (int i = 0; i < 4; i++) grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        for (int i = 0; i < 5; i++) grid.RowStyles.Add(new RowStyle(SizeType.Percent, 20));

        string[,] keys =
        {
            { "CE", "C", "⌫", "÷" },
            { "7",  "8", "9", "×" },
            { "4",  "5", "6", "−" },
            { "1",  "2", "3", "+" },
            { "±",  "0", Dec, "=" }
        };

        var buttonFont = new Font("Segoe UI", 14f);
        for (int r = 0; r < 5; r++)
        {
            for (int c = 0; c < 4; c++)
            {
                string t = keys[r, c];
                var b = new CalcButton
                {
                    Text = t,
                    Dock = DockStyle.Fill,
                    Margin = new Padding(3),
                    FlatStyle = FlatStyle.Flat,
                    Font = buttonFont,
                    BackColor = t switch
                    {
                        "=" => Color.FromArgb(0, 103, 192),
                        "÷" or "×" or "−" or "+" => Color.FromArgb(225, 232, 240),
                        "CE" or "C" or "⌫" or "±" => Color.FromArgb(232, 232, 232),
                        _ => Color.White
                    },
                    ForeColor = t == "=" ? Color.White : Color.Black
                };
                b.FlatAppearance.BorderSize = 0;
                b.Click += (_, _) => OnButton(t);
                grid.Controls.Add(b, c, r);
            }
        }

        root.Controls.Add(expression, 0, 0);
        root.Controls.Add(display, 0, 1);
        root.Controls.Add(grid, 0, 2);
        Controls.Add(root);

        KeyPress += OnFormKeyPress;
    }

    // ---------- Billentyűzet ----------

    private void OnFormKeyPress(object? sender, KeyPressEventArgs e)
    {
        char ch = e.KeyChar;
        if (char.IsAsciiDigit(ch)) OnButton(ch.ToString());
        else if (ch == '.' || ch == ',') OnButton(Dec);
        else if (ch == '+') OnButton("+");
        else if (ch == '-') OnButton("−");
        else if (ch == '*') OnButton("×");
        else if (ch == '/') OnButton("÷");
        else if (ch == '=' || ch == '\r') OnButton("=");
        else if (ch == '\b') OnButton("⌫");
        else if (ch == (char)27) OnButton("C"); // Esc
        else return;

        e.Handled = true;
    }

    // ---------- Gombkezelés ----------

    private void OnButton(string t)
    {
        if (error) Clear();

        switch (t)
        {
            case "C": Clear(); break;
            case "CE": display.Text = "0"; startNewEntry = true; break;
            case "⌫": Backspace(); break;
            case "±": Negate(); break;
            case "+" or "−" or "×" or "÷": Operator(t[0]); break;
            case "=": Equals(); break;
            default:
                if (t == Dec) AppendDecimal();
                else AppendDigit(t);
                break;
        }
    }

    private void Clear()
    {
        display.Text = "0";
        expression.Text = "";
        accumulator = null;
        pendingOp = null;
        startNewEntry = true;
        justEvaluated = false;
        error = false;
    }

    private void AppendDigit(string d)
    {
        if (justEvaluated) Clear(); // "=" után új számolás kezdődik

        if (startNewEntry || display.Text == "0")
        {
            display.Text = d;
            startNewEntry = false;
        }
        else if (display.Text.Count(char.IsDigit) < 15)
        {
            display.Text += d;
        }
    }

    private void AppendDecimal()
    {
        if (justEvaluated) Clear();

        if (startNewEntry)
        {
            display.Text = "0" + Dec;
            startNewEntry = false;
        }
        else if (!display.Text.Contains(Dec))
        {
            display.Text += Dec;
        }
    }

    private void Backspace()
    {
        if (startNewEntry) return;

        string t = display.Text;
        t = t.Length > 1 ? t[..^1] : "0";
        if (t == Neg || t == Neg + "0") t = "0";
        display.Text = t;
    }

    private void Negate()
    {
        if (display.Text == "0") return;

        display.Text = display.Text.StartsWith(Neg, StringComparison.Ordinal)
            ? display.Text[Neg.Length..]
            : Neg + display.Text;
    }

    private void Operator(char op)
    {
        if (pendingOp is null)
        {
            accumulator = ReadDisplay();
        }
        else if (!startNewEntry)
        {
            // láncolt számolás: 2 + 3 + ... → előbb kiszámoljuk a 2 + 3-at
            if (!TryApply(accumulator!.Value, ReadDisplay(), pendingOp.Value, out decimal r)) return;
            accumulator = r;
            display.Text = Format(r);
        }
        // (ha startNewEntry: csak a műveletet cseréljük le)

        pendingOp = op;
        startNewEntry = true;
        justEvaluated = false;
        expression.Text = $"{Format(accumulator!.Value)} {op}";
    }

    private new void Equals()
    {
        if (pendingOp is null || accumulator is null) return;

        decimal right = ReadDisplay();
        string left = Format(accumulator.Value);
        char op = pendingOp.Value;

        if (!TryApply(accumulator.Value, right, op, out decimal result)) return;

        expression.Text = $"{left} {op} {Format(right)} =";
        display.Text = Format(result);
        accumulator = null;
        pendingOp = null;
        startNewEntry = true;
        justEvaluated = true;
    }

    // ---------- Számolás ----------

    private bool TryApply(decimal a, decimal b, char op, out decimal result)
    {
        result = 0;
        try
        {
            switch (op)
            {
                case '+': result = a + b; break;
                case '−': result = a - b; break;
                case '×': result = a * b; break;
                case '÷':
                    if (b == 0)
                    {
                        ShowError("Nullával nem osztható");
                        return false;
                    }
                    result = a / b;
                    break;
            }
            return true;
        }
        catch (OverflowException)
        {
            ShowError("Túl nagy szám");
            return false;
        }
    }

    private void ShowError(string message)
    {
        display.Text = "Hiba";
        expression.Text = message;
        accumulator = null;
        pendingOp = null;
        startNewEntry = true;
        justEvaluated = false;
        error = true;
    }

    private decimal ReadDisplay() =>
        decimal.Parse(display.Text, NumberStyles.Float, CultureInfo.CurrentCulture);

    private static string Format(decimal v)
    {
        if (v == 0) return "0";

        string s = v.ToString("0.##########", CultureInfo.CurrentCulture);
        if (s.Count(char.IsDigit) > 16)
            s = ((double)v).ToString("G10", CultureInfo.CurrentCulture); // pl. 1E+20
        return s;
    }
}