Imports System.ComponentModel

Namespace Models

    Partial Public Class Fattura
        Implements IRevertibleChangeTracking

        Public Sub AcceptChanges() Implements System.ComponentModel.IChangeTracking.AcceptChanges

        End Sub

        Public ReadOnly Property IsChanged As Boolean Implements System.ComponentModel.IChangeTracking.IsChanged
            Get
                Return Nothing
            End Get
        End Property

        Public Sub RejectChanges() Implements System.ComponentModel.IRevertibleChangeTracking.RejectChanges

        End Sub

    End Class

End Namespace