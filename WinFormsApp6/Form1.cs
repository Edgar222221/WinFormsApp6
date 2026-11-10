using System;
using System.Windows.Forms;

namespace WinFormsApp6
{
    public partial class Form1 : Form
    {
        // 1. Változók
        string currentInput = "";        // a most beírt szám
        double result = 0;
        string operation = "";
        bool operationPending = false;
        string expression = "";          // a kijelzőn a most beírt szám előtt álló rész, pl. "2 + 3 + "
        bool resultShown = false;        // "=" után az új számjegy új számot kezd

        public Form1()
        {
            InitializeComponent();
        }

        // Kijelző frissítése: az eddigi kifejezés + a most beírt szám
        private void ShowDisplay()
        {
            textBox1.Text = expression + currentInput;
            textBox1.SelectionStart = textBox1.Text.Length; // hosszú kifejezésnél a végét mutatja
        }

        // 2. Kód a számgombokhoz (0-9)
        private void NumberButton_Click(object sender, EventArgs e)
        {
            if (resultShown)
            {
                currentInput = "";
                expression = "";
                resultShown = false;
            }
            if (currentInput == "0") currentInput = ""; // ne legyen 007

            System.Windows.Forms.Button button = (System.Windows.Forms.Button)sender;
            currentInput += button.Text;
            ShowDisplay();
        }

        // A függőben lévő művelet elvégzése: result (művelet) currentInput.
        // false = hiba történt (nullával osztás).
        private bool Calculate()
        {
            double secondNumber = double.Parse(currentInput);

            switch (operation)
            {
                case "+":
                    result += secondNumber;
                    break;
                case "-":
                    result -= secondNumber;
                    break;
                case "*":
                    result *= secondNumber;
                    break;
                case "/":
                    if (secondNumber == 0)
                    {
                        ClearAll();
                        textBox1.Text = "Hiba";
                        return false;
                    }
                    result /= secondNumber;
                    break;
            }

            result = Math.Round(result, 10);
            return true;
        }

        // 3. Függvény az eredmény kiszámolásához ("=" gomb)
        private void Evaluate()
        {
            if (operationPending && currentInput != "")
            {
                string lastNumber = currentInput;
                if (!Calculate()) return;

                expression += lastNumber + " = ";   // pl. "2 + 3 = "
                currentInput = result.ToString();
                operationPending = false;
                resultShown = true;
                ShowDisplay();                      // pl. "2 + 3 = 5"
            }
        }

        // 4. Kód a műveletekhez (+, -, *, /)
        private void OperationButton_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Button button = (System.Windows.Forms.Button)sender;

            if (currentInput == "")
            {
                // még nincs új szám: csak a műveletet cseréljük le (pl. "5 + " -> "5 - ")
                if (operationPending)
                {
                    operation = button.Text;
                    expression = expression.Substring(0, expression.Length - 2) + button.Text + " ";
                    ShowDisplay();
                }
                return;
            }

            if (operationPending)
            {
                // láncolt számolás: 2 + 3 + ... -> előbb kiszámoljuk a 2 + 3-at
                string lastNumber = currentInput;
                if (!Calculate()) return;

                operation = button.Text;
                expression += lastNumber + " " + operation + " ";
                currentInput = "";
                ShowDisplay();
                return;
            }

            if (resultShown) expression = ""; // "=" után az eredménnyel folytatjuk

            operation = button.Text;
            result = double.Parse(currentInput);
            expression += currentInput + " " + operation + " ";
            currentInput = "";
            operationPending = true;
            resultShown = false;
            ShowDisplay();
        }

        // "=" gomb
        private void EqualsButton_Click(object sender, EventArgs e)
        {
            Evaluate();
        }

        // 5. Kód a Clear gombhoz
        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

        private void ClearAll()
        {
            currentInput = "";
            expression = "";
            result = 0;
            operation = "";
            operationPending = false;
            resultShown = false;
            textBox1.Text = "";
        }

        // ---------- Felület (a Designer kódja ide került, így elég egy fájl) ----------

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            textBox1 = new TextBox();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            button5 = new Button();
            button6 = new Button();
            button7 = new Button();
            button8 = new Button();
            button9 = new Button();
            buttonPlus = new Button();
            button0 = new Button();
            buttonDivide = new Button();
            buttonMinus = new Button();
            buttonMultiply = new Button();
            buttonEquals = new Button();
            buttonClear = new Button();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.BackColor = Color.White;
            textBox1.Font = new Font("Segoe UI", 16F);
            textBox1.Location = new Point(12, 12);
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(268, 36);
            textBox1.TabIndex = 0;
            textBox1.TabStop = false;
            textBox1.TextAlign = HorizontalAlignment.Right;
            // 
            // button1
            // 
            button1.BackColor = Color.White;
            button1.FlatAppearance.BorderColor = Color.Silver;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Segoe UI", 14F);
            button1.Location = new Point(12, 65);
            button1.Name = "button1";
            button1.Size = new Size(84, 52);
            button1.TabIndex = 1;
            button1.Text = "1";
            button1.UseVisualStyleBackColor = false;
            button1.Click += NumberButton_Click;
            // 
            // button2
            // 
            button2.BackColor = Color.White;
            button2.FlatAppearance.BorderColor = Color.Silver;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Font = new Font("Segoe UI", 14F);
            button2.Location = new Point(104, 65);
            button2.Name = "button2";
            button2.Size = new Size(84, 52);
            button2.TabIndex = 2;
            button2.Text = "2";
            button2.UseVisualStyleBackColor = false;
            button2.Click += NumberButton_Click;
            // 
            // button3
            // 
            button3.BackColor = Color.White;
            button3.FlatAppearance.BorderColor = Color.Silver;
            button3.FlatStyle = FlatStyle.Flat;
            button3.Font = new Font("Segoe UI", 14F);
            button3.Location = new Point(196, 65);
            button3.Name = "button3";
            button3.Size = new Size(84, 52);
            button3.TabIndex = 3;
            button3.Text = "3";
            button3.UseVisualStyleBackColor = false;
            button3.Click += NumberButton_Click;
            // 
            // button4
            // 
            button4.BackColor = Color.White;
            button4.FlatAppearance.BorderColor = Color.Silver;
            button4.FlatStyle = FlatStyle.Flat;
            button4.Font = new Font("Segoe UI", 14F);
            button4.Location = new Point(12, 125);
            button4.Name = "button4";
            button4.Size = new Size(84, 52);
            button4.TabIndex = 4;
            button4.Text = "4";
            button4.UseVisualStyleBackColor = false;
            button4.Click += NumberButton_Click;
            // 
            // button5
            // 
            button5.BackColor = Color.White;
            button5.FlatAppearance.BorderColor = Color.Silver;
            button5.FlatStyle = FlatStyle.Flat;
            button5.Font = new Font("Segoe UI", 14F);
            button5.Location = new Point(104, 125);
            button5.Name = "button5";
            button5.Size = new Size(84, 52);
            button5.TabIndex = 5;
            button5.Text = "5";
            button5.UseVisualStyleBackColor = false;
            button5.Click += NumberButton_Click;
            // 
            // button6
            // 
            button6.BackColor = Color.White;
            button6.FlatAppearance.BorderColor = Color.Silver;
            button6.FlatStyle = FlatStyle.Flat;
            button6.Font = new Font("Segoe UI", 14F);
            button6.Location = new Point(196, 125);
            button6.Name = "button6";
            button6.Size = new Size(84, 52);
            button6.TabIndex = 6;
            button6.Text = "6";
            button6.UseVisualStyleBackColor = false;
            button6.Click += NumberButton_Click;
            // 
            // button7
            // 
            button7.BackColor = Color.White;
            button7.FlatAppearance.BorderColor = Color.Silver;
            button7.FlatStyle = FlatStyle.Flat;
            button7.Font = new Font("Segoe UI", 14F);
            button7.Location = new Point(12, 185);
            button7.Name = "button7";
            button7.Size = new Size(84, 52);
            button7.TabIndex = 7;
            button7.Text = "7";
            button7.UseVisualStyleBackColor = false;
            button7.Click += NumberButton_Click;
            // 
            // button8
            // 
            button8.BackColor = Color.White;
            button8.FlatAppearance.BorderColor = Color.Silver;
            button8.FlatStyle = FlatStyle.Flat;
            button8.Font = new Font("Segoe UI", 14F);
            button8.Location = new Point(104, 185);
            button8.Name = "button8";
            button8.Size = new Size(84, 52);
            button8.TabIndex = 8;
            button8.Text = "8";
            button8.UseVisualStyleBackColor = false;
            button8.Click += NumberButton_Click;
            // 
            // button9
            // 
            button9.BackColor = Color.White;
            button9.FlatAppearance.BorderColor = Color.Silver;
            button9.FlatStyle = FlatStyle.Flat;
            button9.Font = new Font("Segoe UI", 14F);
            button9.Location = new Point(196, 185);
            button9.Name = "button9";
            button9.Size = new Size(84, 52);
            button9.TabIndex = 9;
            button9.Text = "9";
            button9.UseVisualStyleBackColor = false;
            button9.Click += NumberButton_Click;
            // 
            // buttonPlus
            // 
            buttonPlus.BackColor = Color.White;
            buttonPlus.FlatAppearance.BorderColor = Color.Silver;
            buttonPlus.FlatStyle = FlatStyle.Flat;
            buttonPlus.Font = new Font("Segoe UI", 14F);
            buttonPlus.Location = new Point(12, 245);
            buttonPlus.Name = "buttonPlus";
            buttonPlus.Size = new Size(84, 52);
            buttonPlus.TabIndex = 10;
            buttonPlus.Text = "+";
            buttonPlus.UseVisualStyleBackColor = false;
            buttonPlus.Click += OperationButton_Click;
            // 
            // button0
            // 
            button0.BackColor = Color.White;
            button0.FlatAppearance.BorderColor = Color.Silver;
            button0.FlatStyle = FlatStyle.Flat;
            button0.Font = new Font("Segoe UI", 14F);
            button0.Location = new Point(104, 245);
            button0.Name = "button0";
            button0.Size = new Size(84, 52);
            button0.TabIndex = 11;
            button0.Text = "0";
            button0.UseVisualStyleBackColor = false;
            button0.Click += NumberButton_Click;
            // 
            // buttonDivide
            // 
            buttonDivide.BackColor = Color.White;
            buttonDivide.FlatAppearance.BorderColor = Color.Silver;
            buttonDivide.FlatStyle = FlatStyle.Flat;
            buttonDivide.Font = new Font("Segoe UI", 14F);
            buttonDivide.Location = new Point(196, 245);
            buttonDivide.Name = "buttonDivide";
            buttonDivide.Size = new Size(84, 52);
            buttonDivide.TabIndex = 12;
            buttonDivide.Text = "/";
            buttonDivide.UseVisualStyleBackColor = false;
            buttonDivide.Click += OperationButton_Click;
            // 
            // buttonMinus
            // 
            buttonMinus.BackColor = Color.White;
            buttonMinus.FlatAppearance.BorderColor = Color.Silver;
            buttonMinus.FlatStyle = FlatStyle.Flat;
            buttonMinus.Font = new Font("Segoe UI", 14F);
            buttonMinus.Location = new Point(12, 305);
            buttonMinus.Name = "buttonMinus";
            buttonMinus.Size = new Size(84, 52);
            buttonMinus.TabIndex = 13;
            buttonMinus.Text = "-";
            buttonMinus.UseVisualStyleBackColor = false;
            buttonMinus.Click += OperationButton_Click;
            // 
            // buttonMultiply
            // 
            buttonMultiply.BackColor = Color.White;
            buttonMultiply.FlatAppearance.BorderColor = Color.Silver;
            buttonMultiply.FlatStyle = FlatStyle.Flat;
            buttonMultiply.Font = new Font("Segoe UI", 14F);
            buttonMultiply.Location = new Point(104, 305);
            buttonMultiply.Name = "buttonMultiply";
            buttonMultiply.Size = new Size(84, 52);
            buttonMultiply.TabIndex = 14;
            buttonMultiply.Text = "*";
            buttonMultiply.UseVisualStyleBackColor = false;
            buttonMultiply.Click += OperationButton_Click;
            // 
            // buttonEquals
            // 
            buttonEquals.BackColor = Color.White;
            buttonEquals.FlatAppearance.BorderColor = Color.Silver;
            buttonEquals.FlatStyle = FlatStyle.Flat;
            buttonEquals.Font = new Font("Segoe UI", 14F);
            buttonEquals.Location = new Point(196, 305);
            buttonEquals.Name = "buttonEquals";
            buttonEquals.Size = new Size(84, 52);
            buttonEquals.TabIndex = 15;
            buttonEquals.Text = "=";
            buttonEquals.UseVisualStyleBackColor = false;
            buttonEquals.Click += EqualsButton_Click;
            // 
            // buttonClear
            // 
            buttonClear.BackColor = Color.White;
            buttonClear.FlatAppearance.BorderColor = Color.Silver;
            buttonClear.FlatStyle = FlatStyle.Flat;
            buttonClear.Font = new Font("Segoe UI", 14F);
            buttonClear.Location = new Point(12, 365);
            buttonClear.Name = "buttonClear";
            buttonClear.Size = new Size(268, 52);
            buttonClear.TabIndex = 16;
            buttonClear.Text = "Clear";
            buttonClear.UseVisualStyleBackColor = false;
            buttonClear.Click += ClearButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(292, 429);
            Controls.Add(buttonClear);
            Controls.Add(buttonEquals);
            Controls.Add(buttonMultiply);
            Controls.Add(buttonMinus);
            Controls.Add(buttonDivide);
            Controls.Add(button0);
            Controls.Add(buttonPlus);
            Controls.Add(button9);
            Controls.Add(button8);
            Controls.Add(button7);
            Controls.Add(button6);
            Controls.Add(button5);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(textBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Balogh Edgár";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button buttonPlus;
        private System.Windows.Forms.Button button0;
        private System.Windows.Forms.Button buttonDivide;
        private System.Windows.Forms.Button buttonMinus;
        private System.Windows.Forms.Button buttonMultiply;
        private System.Windows.Forms.Button buttonEquals;
        private System.Windows.Forms.Button buttonClear;

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}