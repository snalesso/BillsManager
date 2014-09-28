Imports System.Collections.ObjectModel

Imports OldModels.Models
Imports System.Windows.Forms

Namespace Services

    Public Class SuppliersDataService
        Implements ISupplierDataService

        ''' <summary>
        ''' Restituisce un IQueryable(Of Fornitore) di fornitori caricati dal file specificato.
        ''' </summary>
        ''' <param name="DBFile">Path completa del file da cui caricare i fornitori.</param>
        ''' <returns>IQueryable(Of Fornitore)</returns>
        ''' <remarks></remarks>
        Public Function GetAllSuppliers(ByVal DBFile As String) As ObservableCollection(Of Models.Fornitore) Implements ISupplierDataService.GetAllSuppliers

            If Not FileIO.FileSystem.FileExists(DBFile) Then Return Nothing

            Dim xmlDB = XDocument.Load(DBFile)

            Try

                Dim query = From fo In xmlDB...<Fornitori>.Descendants
                             Select New Fornitore(fo.@Città,
                                                  fo.@Ditta,
                                                  fo.@EMail,
                                                  fo.@Indirizzo,
                                                  fo.@Note,
                                                  fo.@Agente,
                                                  fo.@Provincia,
                                                  fo.@Sito,
                                                  fo.@Telefono,
                                                  fo.@TelefonoAgente)

                Return New ObservableCollection(Of Fornitore)(query)

            Catch ex As Exception
                MessageBox.Show("Errore durante il caricamento dei fornitori." &
                                vbCrLf &
                                ex.Message,
                                "Errore di caricamento",
                                Nothing,
                                MessageBoxIcon.Error)
            End Try

            Return Nothing

        End Function

        ''' <summary>
        ''' Salva il gruppo di fornitori specificato in un determinato percorso.
        ''' </summary>
        ''' <param name="Source">Gruppo di fornitori da salvare.</param>
        ''' <param name="DBFile">Path completa del file in cui salvare i fornitori.</param>
        ''' <remarks></remarks>
        Public Sub Save(ByVal Source As ObservableCollection(Of Models.Fornitore),
                        ByVal DBFile As String) Implements ISupplierDataService.Save


            If Source Is Nothing Then Throw New ArgumentNullException("Source", "The value cannot be nothing.")

            Try

                Dim xmlDB = <?xml version="1.0"?>
                            <Fornitori>
                                <%= From fo In Source
                                    Select <Fornitore
                                               Città=<%= fo.Città %>
                                               Ditta=<%= fo.Ditta %>
                                               EMail=<%= fo.EMail %>
                                               Indirizzo=<%= fo.Indirizzo %>
                                               Note=<%= fo.Note %>
                                               Agente=<%= fo.Agente %>
                                               Provincia=<%= fo.Provincia %>
                                               Sito=<%= fo.Sito %>
                                               Telefono=<%= fo.Telefono %>
                                               TelefonoAgente=<%= fo.TelefonoAgente %>/>
                                %>
                            </Fornitori>

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
        ''' Aggiunge un fornitore al gruppo di fornitori specificato.
        ''' </summary>
        ''' <param name="Supplier">Fornitore da aggiungere.</param>
        ''' <param name="Source">Gruppo di fornitori a cui aggiungere il fornitore.</param>
        ''' <remarks></remarks>
        Public Sub Add(ByVal Source As ObservableCollection(Of Models.Fornitore),
                       ByVal Supplier As Models.Fornitore) Implements ISupplierDataService.Add

            If Supplier Is Nothing Then Throw New ArgumentNullException("Source", "The value cannot be nothing.")
            If Source Is Nothing Then Throw New ArgumentNullException("Supplier", "The value cannot be nothing.")

            Source.Add(Supplier)

        End Sub

        ''' <summary>
        ''' Elimina un fornitore dal gruppo di fornitori specificato.
        ''' </summary>
        ''' <param name="Supplier">Fornitore da eliminare.</param>
        ''' <param name="Source">Gruppo di fornitori da cui eliminare il fornitore.</param>
        ''' <remarks></remarks>
        Public Sub Delete(ByVal Source As ObservableCollection(Of Models.Fornitore),
                          ByVal Supplier As Models.Fornitore) Implements ISupplierDataService.Delete

            If Supplier Is Nothing Then Throw New ArgumentNullException("Source", "The value cannot be nothing.")
            If Source Is Nothing Then Throw New ArgumentNullException("Supplier", "The value cannot be nothing.")

            Source.Remove(Supplier)

        End Sub

    End Class

End Namespace