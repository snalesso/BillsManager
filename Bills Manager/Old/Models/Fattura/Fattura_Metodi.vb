'Imports System.ComponentModel

Namespace Models

    Partial Public Class Fattura
        'Implements IRevertibleChangeTracking

#Region " Constructors "

        ''' <summary>
        ''' Istanzia una nuova fattura.
        ''' </summary>
        ''' <remarks></remarks>
        Sub New()
        End Sub

        ''' <summary>
        ''' Istanzia una nuova fattura con i valori specificati.
        ''' </summary>
        ''' <param name="DataCreazione_"></param>
        ''' <param name="DataFattura_"></param>
        ''' <param name="DataScadenza_"></param>
        ''' <param name="DataPagamento_"></param>
        ''' <param name="Fornitore_"></param>
        ''' <param name="Importo_"></param>
        ''' <param name="Note_"></param>
        ''' <param name="Numero_"></param>
        ''' <param name="Pagata_"></param>
        ''' <remarks></remarks>
        Sub New(ByVal DataCreazione_ As Date,
                ByVal DataFattura_ As Date,
                ByVal DataScadenza_ As Date,
                ByVal DataPagamento_ As Date,
                ByVal Fornitore_ As String,
                ByVal Importo_ As Single,
                ByVal Note_ As String,
                ByVal Numero_ As String,
                ByVal Pagata_ As Boolean)

            Me.DataCreazione = DataCreazione_
            Me.DataFattura = DataFattura_
            Me.DataScadenza = DataScadenza_
            Me.DataPagamento = DataPagamento_
            Me.Fornitore = Fornitore_
            Me.Importo = Importo_
            Me.Note = Note_
            Me.Numero = Numero_
            Me.Pagata = Pagata_
        End Sub

        ''' <summary>
        ''' Istanzia una nuova fattura utilizzando i dati di un'altra fattura.
        ''' </summary>
        ''' <param name="Fattura"></param>
        ''' <remarks></remarks>
        Sub New(ByVal Fattura As Fattura)
            Me.DataCreazione = Fattura.DataCreazione
            Me.DataFattura = Fattura.DataFattura
            Me.DataScadenza = Fattura.DataScadenza
            Me.DataPagamento = Fattura.DataPagamento
            Me.Fornitore = Fattura.Fornitore
            Me.Importo = Fattura.Importo
            Me.Note = Fattura.Note
            Me.Numero = Fattura.Numero
            Me.Pagata = Fattura.Pagata
        End Sub

#End Region

#Region " Methods "

        ''' <summary>
        ''' Sostituisce i dati di questa instanza con quelli dell'istanza passata come parametro.
        ''' </summary>
        ''' <param name="Fattura">Fattura da cui copiare i dati.</param>
        ''' <remarks></remarks>
        Public Sub SetFrom(ByVal Fattura As Fattura)
            Me.DataCreazione = Fattura.DataCreazione
            Me.DataFattura = Fattura.DataFattura
            Me.DataScadenza = Fattura.DataScadenza
            Me.DataPagamento = Fattura.DataPagamento
            Me.Fornitore = Fattura.Fornitore
            Me.Importo = Fattura.Importo
            Me.Note = Fattura.Note
            Me.Numero = Fattura.Numero
            Me.Pagata = Fattura.Pagata
        End Sub

#End Region

    End Class

End Namespace