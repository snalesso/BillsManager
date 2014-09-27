Imports System.Collections.ObjectModel

Imports OldModels.Models

Namespace Services

    Public Interface ISupplierDataService

        ''' <summary>
        ''' Restituisce un IQueryable(Of Fornitore) di fornitori caricati dal file specificato.
        ''' </summary>
        ''' <param name="DBFile">Path completa del file da cui caricare i fornitori.</param>
        ''' <returns>IQueryable(Of Fornitore)</returns>
        ''' <remarks></remarks>
        Function GetAllSuppliers(ByVal DBFile As String) As ObservableCollection(Of Fornitore)

        ''' <summary>
        ''' Salva il gruppo di fornitori specificato in un determinato percorso.
        ''' </summary>
        ''' <param name="Source">Gruppo di fornitori da salvare.</param>
        ''' <param name="DBFile">Path completa del file in cui salvare i fornitori.</param>
        ''' <remarks></remarks>
        Sub Save(ByVal Source As ObservableCollection(Of Fornitore),
                 ByVal DBFile As String)

        ''' <summary>
        ''' Aggiunge un fornitore al gruppo di fornitori specificato.
        ''' </summary>
        ''' <param name="Supplier">Fornitore da aggiungere.</param>
        ''' <param name="Source">Gruppo di fornitori a cui aggiungere il fornitore.</param>
        ''' <remarks></remarks>
        Sub Add(ByVal Source As ObservableCollection(Of Fornitore),
                ByVal Supplier As Fornitore)

        ''' <summary>
        ''' Elimina un fornitore dal gruppo di fornitori specificato.
        ''' </summary>
        ''' <param name="Supplier">Fornitore da eliminare.</param>
        ''' <param name="Source">Gruppo di fornitori da cui eliminare il fornitore.</param>
        ''' <remarks></remarks>
        Sub Delete(ByVal Source As ObservableCollection(Of Fornitore),
                   ByVal Supplier As Fornitore)

    End Interface

End Namespace