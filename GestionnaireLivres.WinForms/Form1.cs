using System.ComponentModel;

namespace GestionnaireLivres.WinForms;

public class Form1 : Form
{
    private TextBox txtTitre = null!;
    private TextBox txtAuteur = null!;
    private TextBox txtAnnee = null!;
    private ComboBox cmbGenre = null!;
    private CheckBox chkLu = null!;
    private Button btnAjouter = null!;
    private Button btnModifier = null!;
    private Button btnSupprimer = null!;
    private DataGridView dgvLivres = null!;

    private BindingList<Livre> livres = new();
    private Livre? livreEnEdition = null;

    public Form1()
    {
        InitialiserInterface();
    }

    private void InitialiserInterface()
    {
        this.Text = "Gestionnaire de Livres";
        this.Size = new Size(900, 650);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.MinimumSize = new Size(800, 550);

        var panelSaisie = new Panel
        {
            Dock = DockStyle.Top,
            Height = 240,
            Padding = new Padding(15)
        };

        int controlLeft = 100;
        int controlWidth = 250;
        int yPos = 15;
        int rowHeight = 35;

        var lblTitre = new Label { Text = "Titre :", Location = new Point(15, yPos + 3), AutoSize = true };
        txtTitre = new TextBox { Location = new Point(controlLeft, yPos), Width = controlWidth };
        yPos += rowHeight;

        var lblAuteur = new Label { Text = "Auteur :", Location = new Point(15, yPos + 3), AutoSize = true };
        txtAuteur = new TextBox { Location = new Point(controlLeft, yPos), Width = controlWidth };
        yPos += rowHeight;

        var lblAnnee = new Label { Text = "Année :", Location = new Point(15, yPos + 3), AutoSize = true };
        txtAnnee = new TextBox { Location = new Point(controlLeft, yPos), Width = controlWidth };
        yPos += rowHeight;

        var lblGenre = new Label { Text = "Genre :", Location = new Point(15, yPos + 3), AutoSize = true };
        cmbGenre = new ComboBox
        {
            Location = new Point(controlLeft, yPos),
            Width = controlWidth,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbGenre.Items.AddRange(new object[] { "", "Roman", "SF", "Fantasy", "Policier", "Autre" });
        cmbGenre.SelectedIndex = 0;
        yPos += rowHeight;

        chkLu = new CheckBox
        {
            Text = "Lu ?",
            Location = new Point(controlLeft, yPos),
            AutoSize = true
        };
        yPos += rowHeight;

        int btnY = yPos;
        btnAjouter = new Button
        {
            Text = "Ajouter",
            Location = new Point(controlLeft, btnY),
            Width = 120,
            Height = 35,
            BackColor = Color.FromArgb(46, 139, 87),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        btnAjouter.Click += BtnAjouter_Click;

        btnModifier = new Button
        {
            Text = "Modifier",
            Location = new Point(controlLeft + 130, btnY),
            Width = 120,
            Height = 35,
            BackColor = Color.FromArgb(70, 130, 180),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Enabled = false
        };
        btnModifier.Click += BtnModifier_Click;

        btnSupprimer = new Button
        {
            Text = "Supprimer",
            Location = new Point(controlLeft + 260, btnY),
            Width = 120,
            Height = 35,
            BackColor = Color.FromArgb(205, 92, 92),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Enabled = false
        };
        btnSupprimer.Click += BtnSupprimer_Click;

        var btnAnnuler = new Button
        {
            Text = "Annuler",
            Location = new Point(controlLeft + 390, btnY),
            Width = 120,
            Height = 35,
            FlatStyle = FlatStyle.Flat
        };
        btnAnnuler.Click += BtnAnnuler_Click;

        panelSaisie.Controls.AddRange(new Control[]
        {
            lblTitre, txtTitre,
            lblAuteur, txtAuteur,
            lblAnnee, txtAnnee,
            lblGenre, cmbGenre,
            chkLu,
            btnAjouter, btnModifier, btnSupprimer, btnAnnuler
        });

        dgvLivres = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.Fixed3D
        };
        dgvLivres.DataSource = livres;
        dgvLivres.CellDoubleClick += DgvLivres_CellDoubleClick;
        dgvLivres.SelectionChanged += DgvLivres_SelectionChanged;

        dgvLivres.DataBindingComplete += (s, e) =>
        {
            if (dgvLivres.Columns.Count > 0)
            {
                dgvLivres.Columns["Titre"].HeaderText = "Titre";
                dgvLivres.Columns["Auteur"].HeaderText = "Auteur";
                dgvLivres.Columns["Annee"].HeaderText = "Année";
                dgvLivres.Columns["Genre"].HeaderText = "Genre";
                dgvLivres.Columns["Lu"].HeaderText = "Lu";
            }
        };

        this.Controls.Add(dgvLivres);
        this.Controls.Add(panelSaisie);
    }

    private bool ValiderFormulaire(out string erreurs)
    {
        var listeErreurs = new List<string>();

        if (string.IsNullOrWhiteSpace(txtTitre.Text) || txtTitre.Text.Trim().Length < 2)
            listeErreurs.Add("• Titre : obligatoire, minimum 2 caractères");

        if (string.IsNullOrWhiteSpace(txtAuteur.Text) || txtAuteur.Text.Trim().Length < 2)
            listeErreurs.Add("• Auteur : obligatoire, minimum 2 caractères");

        if (!int.TryParse(txtAnnee.Text, out int annee) || annee < 1800 || annee > DateTime.Now.Year)
            listeErreurs.Add($"• Année : doit être un entier entre 1800 et {DateTime.Now.Year}");

        if (cmbGenre.SelectedIndex <= 0)
            listeErreurs.Add("• Genre : vous devez sélectionner un genre");

        erreurs = string.Join("\n", listeErreurs);
        return listeErreurs.Count == 0;
    }

    private void BtnAjouter_Click(object? sender, EventArgs e)
    {
        if (!ValiderFormulaire(out string erreurs))
        {
            MessageBox.Show(erreurs, "Erreurs de validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var livre = new Livre
        {
            Titre = txtTitre.Text.Trim(),
            Auteur = txtAuteur.Text.Trim(),
            Annee = int.Parse(txtAnnee.Text),
            Genre = cmbGenre.SelectedItem!.ToString()!,
            Lu = chkLu.Checked
        };

        livres.Add(livre);
        ViderFormulaire();
    }

    private void BtnModifier_Click(object? sender, EventArgs e)
    {
        if (livreEnEdition == null) return;

        if (!ValiderFormulaire(out string erreurs))
        {
            MessageBox.Show(erreurs, "Erreurs de validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        livreEnEdition.Titre = txtTitre.Text.Trim();
        livreEnEdition.Auteur = txtAuteur.Text.Trim();
        livreEnEdition.Annee = int.Parse(txtAnnee.Text);
        livreEnEdition.Genre = cmbGenre.SelectedItem!.ToString()!;
        livreEnEdition.Lu = chkLu.Checked;

        livres.ResetBindings();
        ViderFormulaire();
        livreEnEdition = null;
        btnModifier.Enabled = false;
        btnAjouter.Enabled = true;
    }

    private void BtnSupprimer_Click(object? sender, EventArgs e)
    {
        if (dgvLivres.CurrentRow == null) return;

        var livre = (Livre)dgvLivres.CurrentRow.DataBoundItem;
        var result = MessageBox.Show(
            $"Voulez-vous vraiment supprimer le livre \"{livre.Titre}\" ?",
            "Confirmation de suppression",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            livres.Remove(livre);
            ViderFormulaire();
            livreEnEdition = null;
            btnModifier.Enabled = false;
            btnAjouter.Enabled = true;
        }
    }

    private void DgvLivres_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;

        livreEnEdition = (Livre)dgvLivres.Rows[e.RowIndex].DataBoundItem;

        txtTitre.Text = livreEnEdition.Titre;
        txtAuteur.Text = livreEnEdition.Auteur;
        txtAnnee.Text = livreEnEdition.Annee.ToString();
        cmbGenre.SelectedItem = livreEnEdition.Genre;
        chkLu.Checked = livreEnEdition.Lu;

        btnModifier.Enabled = true;
        btnAjouter.Enabled = false;
    }

    private void DgvLivres_SelectionChanged(object? sender, EventArgs e)
    {
        btnSupprimer.Enabled = dgvLivres.CurrentRow != null && dgvLivres.Rows.Count > 0;
    }

    private void BtnAnnuler_Click(object? sender, EventArgs e)
    {
        ViderFormulaire();
        livreEnEdition = null;
        btnModifier.Enabled = false;
        btnAjouter.Enabled = true;
    }

    private void ViderFormulaire()
    {
        txtTitre.Text = string.Empty;
        txtAuteur.Text = string.Empty;
        txtAnnee.Text = string.Empty;
        cmbGenre.SelectedIndex = 0;
        chkLu.Checked = false;
    }
}
