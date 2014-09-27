Namespace Models

    Partial Public Class Fattura
        Implements IComparable(Of Fattura)

        Public Function CompareTo(ByVal other As Fattura) As Integer Implements System.IComparable(Of Fattura).CompareTo
            Dim bc As New BillComparer
            Return bc.Compare(Me, other)
        End Function

    End Class

End Namespace