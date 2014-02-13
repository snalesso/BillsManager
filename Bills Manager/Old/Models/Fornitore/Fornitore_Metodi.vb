Namespace Models

    Public Class Fornitore

#Region " Costruttori "

        ''' <summary>
        ''' Istanzia un nuovo fornitore.
        ''' </summary>
        ''' <remarks></remarks>
        Sub New()
        End Sub

        ''' <summary>
        ''' Istanzia un nuovo fornitore utilizzando i parametri specificati.
        ''' </summary>
        ''' <param name="Città_"></param>
        ''' <param name="Ditta_"></param>
        ''' <param name="EMail_"></param>
        ''' <param name="Indir_"></param>
        ''' <param name="Note_"></param>
        ''' <param name="Agente"></param>
        ''' <param name="Prov_"></param>
        ''' <param name="Sito_"></param>
        ''' <param name="Tel_"></param>
        ''' <param name="TelAgente"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal Città_ As String,
                       ByVal Ditta_ As String,
                       ByVal EMail_ As String,
                       ByVal Indir_ As String,
                       ByVal Note_ As String,
                       ByVal agente As String,
                       ByVal Prov_ As String,
                       ByVal Sito_ As String,
                       ByVal Tel_ As String,
                       ByVal TelAgente As String)
            Me.Città = Città_
            Me.Ditta = Ditta_
            Me.EMail = EMail_
            Me.Sito = Sito_
            Me.Indirizzo = Indir_
            Me.Note = Note_
            Me.Agente = agente
            Me.Provincia = Prov_
            Me.Telefono = Tel_
            Me.TelefonoAgente = TelAgente
        End Sub

        ''' <summary>
        ''' Istanzia un nuovo fornitore utilizzando i dati di un'altro fornitore.
        ''' </summary>
        ''' <param name="Fornitore"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal Fornitore As Fornitore)
            If Fornitore IsNot Nothing Then
                With Me
                    .Città = Fornitore.Città
                    .Ditta = Fornitore.Ditta
                    .EMail = Fornitore.EMail
                    .Indirizzo = Fornitore.Indirizzo
                    .Note = Fornitore.Note
                    .Agente = Fornitore.Agente
                    .Provincia = Fornitore.Provincia
                    .Saldo = Fornitore.Saldo
                    .Sito = Fornitore.Sito
                    .Telefono = Fornitore.Telefono
                    .TelefonoAgente = Fornitore.TelefonoAgente
                End With
            End If
        End Sub

#End Region

        ''' <summary>
        ''' Ritorna un nuovo fornitore che presenta gli stessi dati dell'istanza da cui viene creato.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Clone() As Fornitore
            Return New Fornitore(Me)
        End Function

        ''' <summary>
        ''' Sostituisce i dati di questa instanza con quelli dell'istanza passata come parametro.
        ''' </summary>
        ''' <param name="Fornitore">Fornitore da cui copiare i dati.</param>
        ''' <remarks></remarks>
        Public Sub SetFrom(ByVal Fornitore As Fornitore)
            Me.Città = Fornitore.Città
            Me.Ditta = Fornitore.Ditta
            Me.EMail = Fornitore.EMail
            Me.Indirizzo = Fornitore.Indirizzo
            Me.Note = Fornitore.Note
            Me.Agente = Fornitore.Agente
            Me.Provincia = Fornitore.Provincia
            Me.Saldo = Fornitore.Saldo
            Me.Sito = Fornitore.Sito
            Me.Telefono = Fornitore.Telefono
            Me.TelefonoAgente = Fornitore.TelefonoAgente
        End Sub

    End Class

End Namespace