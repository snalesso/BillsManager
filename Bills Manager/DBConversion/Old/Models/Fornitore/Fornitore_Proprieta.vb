Imports Old.MVVM

Namespace Models

    Public Enum Obbligazione
        Nulla = 0

        'Il fornitore è in credito
        Avanza = 1

        'Il fornitore è in debito
        Deve = 2
    End Enum

    Partial Public Class Fornitore
        Inherits PropertyChanged

        Dim _città As String
        Public Property Città As String
            Get
                Return _città
            End Get
            Set(ByVal value As String)
                _città = value
                OnPropertyChanged("Città")
            End Set
        End Property

        Dim _ditta As String
        Public Property Ditta As String
            Get
                Return _ditta
            End Get
            Set(ByVal value As String)
                _ditta = value
                OnPropertyChanged("Ditta")
            End Set
        End Property

        Dim _eMail As String
        Public Property EMail As String
            Get
                Return _eMail
            End Get
            Set(ByVal value As String)
                _eMail = value
                OnPropertyChanged("EMail")
            End Set
        End Property

        Dim _indirizzo As String
        Public Property Indirizzo As String
            Get
                Return _indirizzo
            End Get
            Set(ByVal value As String)
                _indirizzo = value
                OnPropertyChanged("Indirizzo")
            End Set
        End Property

        Dim _note As String
        Public Property Note As String
            Get
                Return _note
            End Get
            Set(ByVal value As String)
                _note = value
                OnPropertyChanged("Note")
            End Set
        End Property

        Dim _agente As String
        Public Property Agente As String
            Get
                Return _agente
            End Get
            Set(ByVal value As String)
                _agente = value
                OnPropertyChanged("Agente")
            End Set
        End Property

        Public ReadOnly Property Obbligazione As Obbligazione
            Get
                If Saldo > 0 Then Return Models.Obbligazione.Avanza
                If Saldo < 0 Then Return Models.Obbligazione.Deve
                Return Models.Obbligazione.Nulla
            End Get
        End Property

        Dim _provincia As String
        Public Property Provincia As String
            Get
                Return _provincia
            End Get
            Set(ByVal value As String)
                _provincia = value
                OnPropertyChanged("Provincia")
            End Set
        End Property

        Dim _saldo As Double
        Public Property Saldo As Double
            Get
                Return _saldo
            End Get
            Set(ByVal value As Double)
                _saldo = value
                OnPropertyChanged("Saldo")
                OnPropertyChanged("Obbligazione")
            End Set
        End Property

        Dim _sito As String
        Public Property Sito As String
            Get
                Return _sito
            End Get
            Set(ByVal value As String)
                _sito = value
                OnPropertyChanged("Sito")
            End Set
        End Property

        Dim _telefono As String
        Public Property Telefono As String
            Get
                Return _telefono
            End Get
            Set(ByVal value As String)
                _telefono = value
                OnPropertyChanged("Telefono")
            End Set
        End Property

        Dim _telefonoAgente As String
        Public Property TelefonoAgente As String
            Get
                Return _telefonoAgente
            End Get
            Set(ByVal value As String)
                _telefonoAgente = value
                OnPropertyChanged("TelefonoAgente")
            End Set
        End Property

    End Class

End Namespace