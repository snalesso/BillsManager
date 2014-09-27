Namespace Models

    Public Class FatturaComparer
        Implements IComparer(Of Fattura)

        Public Function Compare(ByVal x As Fattura, ByVal y As Fattura) As Integer Implements System.Collections.Generic.IComparer(Of Fattura).Compare

            If x.DataScadenza < y.DataScadenza Then Return -1

            If x.DataScadenza = y.DataScadenza Then

                If (Not x.Pagata) And y.Pagata Then Return -1

                If x.Pagata = y.Pagata Then

                    If x.Numero < y.Numero Then Return -1

                    If x.Numero = y.Numero Then Return 0

                    Return 1

                End If

                Return -1

            End If

            Return 1

        End Function

    End Class

End Namespace