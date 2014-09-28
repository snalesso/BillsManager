Namespace Models

    Public Class BillComparer
        Implements IComparer(Of Fattura)

        Public Function Compare(ByVal x As Fattura, ByVal y As Fattura) As Integer Implements System.Collections.Generic.IComparer(Of Fattura).Compare
            If x.Pagata > y.Pagata Then
                Return 1
            ElseIf x.Pagata = y.Pagata Then
                Return 0
            Else
                Return -1
            End If
        End Function

    End Class

End Namespace