Namespace Models

    Public Class FornitoreComparer
        Implements IComparer(Of Fornitore)

        Public Function Compare(ByVal x As Fornitore, ByVal y As Fornitore) As Integer Implements System.Collections.Generic.IComparer(Of Fornitore).Compare

            If x.Ditta < y.Ditta Then Return -1

            If x.Ditta = y.Ditta Then

                If x.Città < y.Città Then Return -1

                If x.Città = y.Città Then Return 0

                Return 1

            End If

            Return 1

        End Function

    End Class

End Namespace