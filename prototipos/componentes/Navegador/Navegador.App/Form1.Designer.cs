namespace Navegador.App.Navegador_Capa_Vista;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        btn_anterior = new Button();
        btn_siguiente = new Button();
        btn_fin = new Button();
        btn_refrescar = new Button();
        dgv_datos = new DataGridView();
        ((System.ComponentModel.ISupportInitialize)dgv_datos).BeginInit();
        SuspendLayout();
        // 
        // btn_anterior
        // 
        btn_anterior.Location = new Point(100, 337);
        btn_anterior.Name = "btn_anterior";
        btn_anterior.Size = new Size(75, 23);
        btn_anterior.TabIndex = 0;
        btn_anterior.Text = "Anterior";
        btn_anterior.UseVisualStyleBackColor = true;
        // 
        // btn_siguiente
        // 
        btn_siguiente.Location = new Point(582, 337);
        btn_siguiente.Name = "btn_siguiente";
        btn_siguiente.Size = new Size(75, 23);
        btn_siguiente.TabIndex = 1;
        btn_siguiente.Text = "Siguiente";
        btn_siguiente.UseVisualStyleBackColor = true;
        // 
        // btn_fin
        // 
        btn_fin.Location = new Point(256, 337);
        btn_fin.Name = "btn_fin";
        btn_fin.Size = new Size(75, 23);
        btn_fin.TabIndex = 2;
        btn_fin.Text = "Ultimo";
        btn_fin.UseVisualStyleBackColor = true;
        // 
        // btn_refrescar
        // 
        btn_refrescar.Location = new Point(412, 337);
        btn_refrescar.Name = "btn_refrescar";
        btn_refrescar.Size = new Size(75, 23);
        btn_refrescar.TabIndex = 3;
        btn_refrescar.Text = "Recargar";
        btn_refrescar.UseVisualStyleBackColor = true;
        // 
        // dgv_datos
        // 
        dgv_datos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgv_datos.Location = new Point(269, 108);
        dgv_datos.Name = "dgv_datos";
        dgv_datos.Size = new Size(240, 150);
        dgv_datos.TabIndex = 4;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 450);
        Controls.Add(dgv_datos);
        Controls.Add(btn_refrescar);
        Controls.Add(btn_fin);
        Controls.Add(btn_siguiente);
        Controls.Add(btn_anterior);
        Name = "Form1";
        Text = "Form1";
        ((System.ComponentModel.ISupportInitialize)dgv_datos).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private Button btn_anterior;
    private Button btn_siguiente;
    private Button btn_fin;
    private Button btn_refrescar;
    private DataGridView dgv_datos;
}
