Imports System.Collections.ObjectModel

Imports Old.Models

Namespace Services

    Public Interface IBillDataService

        ''' <summary>
        ''' Restituisce un IQueryable(Of Fattura) di fatture caricate dal file specificato.
        ''' </summary>
        ''' <param name="DBFile">Path completa del file da cui caricare le fatture.</param>
        ''' <returns>IQueryable(Of Fattura)</returns>
        ''' <remarks></remarks>
        Function GetAllBills(ByVal DBFile As String) As ObservableCollection(Of Fattura)

        ''' <summary>
        ''' Salva il gruppo di fatture specificato in un determinato percorso.
        ''' </summary>
        ''' <param name="Source">Gruppo di fatture da salvare.</param>
        ''' <param name="DBFile">Path completa del file in cui salvare le fatture.</param>
        ''' <remarks></remarks>
        Sub Save(ByVal Source As ObservableCollection(Of Fattura),
                 ByVal DBFile As String)

        ''' <summary>
        ''' Aggiunge una fattura al gruppo di fatture specificato.
        ''' </summary>
        ''' <param name="Bill">Fattura da aggiungere.</param>
        ''' <param name="Source">Gruppo di fatture a cui aggiungere la fattura.</param>
        ''' <remarks></remarks>
        Sub Add(ByVal Source As ObservableCollection(Of Fattura),
                ByVal Bill As Fattura)

        ''' <summary>
        ''' Elimina una fattura dal gruppo di fatture specificato.
        ''' </summary>
        ''' <param name="Bill">Fattura da eliminare.</param>
        ''' <param name="Source">Gruppo di fatture da cui eliminare la fattura.</param>
        ''' <remarks></remarks>
        Sub Delete(ByVal Source As ObservableCollection(Of Fattura),
                   ByVal Bill As Fattura)

    End Interface

End Namespace