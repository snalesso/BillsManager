Imports System.Collections.ObjectModel

Imports Old.Models
Imports System.Windows.Forms

Namespace Services

    Public Class BillsDataService
        Implements IBillDataService

        ''' <summary>
        ''' Restituisce un IQueryable(Of Fattura) di fatture caricate dal file specificato.
        ''' </summary>
        ''' <param name="DBFile">Path completa del file da cui caricare le fatture.</param>
        ''' <returns>IQueryable(Of Fattura)</returns>
        ''' <remarks></remarks>
        Public Function GetAllBills(ByVal DBFile As String) As ObservableCollection(Of Models.Fattura) Implements IBillDataService.GetAllBills

            If Not FileIO.FileSystem.FileExists(DBFile) Then Return Nothing

            Dim xmlDB = XDocument.Load(DBFile)

            Try

                Dim query = From fa In xmlDB...<Fatture>.Descendants
                            Select New Fattura(fa.@DataCreazione,
                                               fa.@DataFattura,
                                               fa.@DataScadenza,
                                               IIf(fa.@DataPagamento = String.Empty,
                                                   Nothing,
                                                   fa.@DataPagamento),
                                               fa.@Fornitore,
                                               fa.@Importo,
                                               fa.@Note,
                                               fa.@Numero,
                                               fa.@Pagata)

                Return New ObservableCollection(Of Fattura)(query)

            Catch ex As Exception
                MessageBox.Show("Errore durante il caricamento delle fatture." &
                                vbCrLf &
                                ex.Message,
                                "Errore di caricamento",
                                Nothing,
                                MessageBoxIcon.Error)
            End Try

            Return Nothing

        End Function

        ''' <summary>
        ''' Salva il gruppo di fatture specificato in un determinato percorso.
        ''' </summary>
        ''' <param name="Source">Gruppo di fatture da salvare.</param>
        ''' <param name="DBFile">Path completa del file in cui salvare le fatture.</param>
        ''' <remarks></remarks>
        Public Sub Save(ByVal Source As ObservableCollection(Of Models.Fattura),
                        ByVal DBFile As String) Implements IBillDataService.Save

            If Source Is Nothing Then Throw New ArgumentNullException("Source", "The value cannot be nothing.")

            Try

                Dim xmlDB = <?xml version="1.0"?>
                            <Fatture>
                                <%= From fa In Source
                                    Select <Fattura
                                               DataCreazione=<%= fa.DataCreazione %>
                                               DataFattura=<%= fa.DataFattura %>
                                               DataScadenza=<%= fa.DataScadenza %>
                                               DataPagamento=<%= fa.DataPagamento %>
                                               Fornitore=<%= fa.Fornitore %>
                                               Importo=<%= fa.Importo.ToString.Replace(".", ",") %>
                                               Note=<%= fa.Note %>
                                               Numero=<%= fa.Numero %>
                                               Pagata=<%= fa.Pagata %>/>
                                %>
                            </Fatture>

                If Not FileIO.FileSystem.DirectoryExists(AppDomain.CurrentDomain.BaseDirectory & "DB") Then _
                    FileIO.FileSystem.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory & "DB")

                xmlDB.Save(DBFile)

            Catch ex As Exception
                MessageBox.Show("Errore durante il salvataggio delle fatture." &
                                vbCrLf &
                                ex.Message,
                                "Errore di salvataggio",
                                Nothing,
                                MessageBoxIcon.Error)
            End Try
        End Sub

        ''' <summary>
        ''' Aggiunge una fattura al gruppo di fatture specificato.
        ''' </summary>
        ''' <param name="Bill">Fattura da aggiungere.</param>
        ''' <param name="Source">Gruppo di fatture a cui aggiungere la fattura.</param>
        ''' <remarks></remarks>
        Public Sub Add(ByVal Source As ObservableCollection(Of Models.Fattura),
                       ByVal Bill As Models.Fattura) Implements IBillDataService.Add

            If Source Is Nothing Then Throw New ArgumentNullException("Source", "The value cannot be nothing.")
            If Bill Is Nothing Then Throw New ArgumentNullException("Bill", "The value cannot be nothing.")

            Source.Add(Bill)

        End Sub

        ''' <summary>
        ''' Elimina una fattura dal gruppo di fatture specificato.
        ''' </summary>
        ''' <param name="Bill">Fattura da eliminare.</param>
        ''' <param name="Source">Gruppo di fatture da cui eliminare la fattura.</param>
        ''' <remarks></remarks>
        Public Sub Delete(ByVal Source As ObservableCollection(Of Models.Fattura),
                          ByVal Bill As Models.Fattura) Implements IBillDataService.Delete

            If Source Is Nothing Then Throw New ArgumentNullException("Source", "The value cannot be nothing.")
            If Bill Is Nothing Then Throw New ArgumentNullException("Bill", "The value cannot be nothing.")

            Source.Remove(Bill)

        End Sub

    End Class

End Namespace