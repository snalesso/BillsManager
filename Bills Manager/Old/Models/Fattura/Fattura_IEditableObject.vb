Imports System.ComponentModel

Namespace Models

    Partial Public Class Fattura
        Implements IEditableObject

        Private _backup As Fattura

        'Private _isInEditMode As Boolean
        'Public Property IsInEditMode As Boolean
        '    Get
        '        Return _isInEditMode
        '    End Get
        '    Set(ByVal value As Boolean)
        '        If _isInEditMode <> value Then
        '            _isInEditMode = value
        '            OnPropertyChanged("IsInEditMode")
        '        End If
        '    End Set
        'End Property

        Public Sub BeginEdit() Implements System.ComponentModel.IEditableObject.BeginEdit
            _backup = New Fattura With {.DataCreazione = Me.DataCreazione,
                                        .DataFattura = Me.DataFattura,
                                        .DataPagamento = Me.DataPagamento,
                                        .DataScadenza = Me.DataScadenza,
                                        .Fornitore = Me.Fornitore,
                                        .Importo = Me.Importo,
                                        .Note = Me.Note,
                                        .Numero = Me.Numero,
                                        .Pagata = Me.Pagata}
            '_isInEditMode = True
        End Sub

        Public Sub CancelEdit() Implements System.ComponentModel.IEditableObject.CancelEdit
            Me.DataCreazione = _backup.DataCreazione
            Me.DataFattura = _backup.DataFattura
            Me.DataPagamento = _backup.DataPagamento
            Me.DataScadenza = _backup.DataScadenza
            Me.Fornitore = _backup.Fornitore
            Me.Importo = _backup.Importo
            Me.Note = _backup.Note
            Me.Numero = _backup.Numero
            Me.Pagata = _backup.Pagata

            _backup = Nothing

            '_isInEditMode = False
        End Sub

        Public Sub EndEdit() Implements System.ComponentModel.IEditableObject.EndEdit
            _backup = Nothing
            '_isInEditMode = False
        End Sub

    End Class

End Namespace