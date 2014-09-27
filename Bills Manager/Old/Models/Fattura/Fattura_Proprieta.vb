Imports OldModels.MVVM

Namespace Models

    Partial Public Class Fattura
        Inherits PropertyChanged

        Private _DataCreazione As Date = Now
        Public Property DataCreazione As Date
            Get
                Return _DataCreazione
            End Get
            Set(ByVal v As Date)
                _DataCreazione = v
                OnPropertyChanged("DataCreazione")
            End Set
        End Property

        Private _DataFattura As Date = Today
        Public Property DataFattura As Date
            Get
                Return _DataFattura
            End Get
            Set(ByVal v As Date)
                _DataFattura = v
                OnPropertyChanged("DataFattura")
                'If DataScadenza < DataFattura Then DataScadenza = DataFattura
            End Set
        End Property

        Private _DataPagamento As Date? = Nothing
        Public Property DataPagamento As Date?
            Get
                'If (Pagata And _DataPagamento.HasValue) Then
                Return _DataPagamento
                'End If
                'Return Nothing
                'Return _DataPagamento
            End Get
            Set(ByVal value As Date?)
                _DataPagamento = value
                OnPropertyChanged("DataPagamento")
            End Set
        End Property

        Private _DataScadenza As Date = Today
        Public Property DataScadenza As Date
            Get
                Return _DataScadenza
            End Get
            Set(ByVal v As Date)
                _DataScadenza = v
                OnPropertyChanged("DataScadenza")
                OnPropertyChanged("Scaduta")

                'If DataFattura > DataScadenza Then DataFattura = DataScadenza

                OnPropertyChanged("ColoreScadenza")
                OnPropertyChanged("GiorniRimasti")
            End Set
        End Property

        Public ReadOnly Property GiorniRimasti As String
            Get
                If Pagata Then
                    Return String.Empty
                Else
                    Dim n = _DataScadenza.Subtract(Today).TotalDays
                    Select Case n
                        Case Is > 0
                            Return "Scade tra " & n & " giorni"

                        Case Is = 0
                            Return "Scade oggi"

                        Case Else
                            Return "Scaduta " & (n * -1).ToString & " giorni fa"

                    End Select
                End If
            End Get
        End Property

        Private _Fornitore As String
        Public Property Fornitore As String
            Get
                Return _Fornitore
            End Get
            Set(ByVal value As String)
                _Fornitore = value
                OnPropertyChanged("Fornitore")
            End Set
        End Property

        Private _Importo As Single
        Public Property Importo As Single
            Get
                Return _Importo
            End Get
            Set(ByVal v As Single)
                _Importo = v
                OnPropertyChanged("Importo")
                OnPropertyChanged("Saldo")
                OnPropertyChanged("NotaDiCredito")
            End Set
        End Property

        Private _Note As String
        Public Property Note As String '( = String.Empty)
            Get
                Return _Note
            End Get
            Set(ByVal v As String)
                _Note = v
                OnPropertyChanged("Note")
            End Set
        End Property

        Public ReadOnly Property NotaDiCredito As Boolean
            Get
                Return (Importo < 0)
            End Get
        End Property

        Private _Numero As String
        Public Property Numero As String
            Get
                Return _Numero
            End Get
            Set(ByVal value As String)
                _Numero = value
                OnPropertyChanged("Numero")
            End Set
        End Property

        Private _Pagata As Boolean
        Public Property Pagata As Boolean
            Get
                Return _Pagata
            End Get
            Set(ByVal value As Boolean)
                _Pagata = value
                OnPropertyChanged("Pagata")

                If value Then
                    If Not DataPagamento.HasValue Then
                        DataPagamento = Today
                        'OnPropertyChanged("DataPagamento")
                        'Debug.Print("Impostato oggi")
                    End If
                Else
                    DataPagamento = Nothing
                    'OnPropertyChanged("DataPagamento")
                    'Debug.Print("Impostato nothing")
                End If

                'End If

                OnPropertyChanged("Saldo")
                OnPropertyChanged("ColoreScadenza")
            End Set
        End Property

        Public ReadOnly Property Saldo As Single
            Get
                If Not Pagata Then Return 0
                Return _Importo
            End Get
        End Property

        Public ReadOnly Property Scaduta As Boolean
            Get
                Return DataScadenza < Today
            End Get
        End Property

    End Class

End Namespace