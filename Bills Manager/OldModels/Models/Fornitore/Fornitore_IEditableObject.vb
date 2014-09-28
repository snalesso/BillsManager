Imports System.ComponentModel

Namespace Models

    Partial Public Class Fornitore
        Implements IEditableObject

        Private _backup As Fornitore

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
            _backup = New Fornitore With {.Città = Me.Città,
                                          .Ditta = Me.Ditta,
                                          .EMail = Me.EMail,
                                          .Indirizzo = Me.Indirizzo,
                                          .Note = Me.Note,
                                          .Agente = Me.Agente,
                                          .Provincia = Me.Provincia,
                                          .Sito = Me.Sito,
                                          .Telefono = Me.Telefono,
                                          .TelefonoAgente = Me.TelefonoAgente}
            '_isInEditMode = True
        End Sub

        Public Sub CancelEdit() Implements System.ComponentModel.IEditableObject.CancelEdit
            Me.Città = _backup.Città
            Me.Ditta = _backup.Ditta
            Me.EMail = _backup.EMail
            Me.Indirizzo = _backup.Indirizzo
            Me.Note = _backup.Note
            Me.Agente = _backup.Agente
            Me.Provincia = _backup.Provincia
            Me.Sito = _backup.Sito
            Me.Telefono = _backup.Telefono
            Me.TelefonoAgente = _backup.TelefonoAgente

            _backup = Nothing

            '_isInEditMode = False
        End Sub

        Public Sub EndEdit() Implements System.ComponentModel.IEditableObject.EndEdit
            _backup = Nothing
            '_isInEditMode = False
        End Sub

    End Class

End Namespace