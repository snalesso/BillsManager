Imports System.ComponentModel

Namespace MVVM

    Public MustInherit Class PropertyChanged
        Implements INotifyPropertyChanged

        Public Event PropertyChanged(ByVal sender As Object,
                                     ByVal e As System.ComponentModel.PropertyChangedEventArgs) _
                                 Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged

        Protected Sub OnPropertyChanged(ByVal strPropertyName As String)
            If Me.PropertyChangedEvent IsNot Nothing Then
                RaiseEvent PropertyChanged(Me,
                                           New System.ComponentModel.PropertyChangedEventArgs(strPropertyName))
            End If
        End Sub

        Private privateThrowOnInvalidPropertyName As Boolean
        Protected Overridable Property ThrowOnInvalidPropertyName() As Boolean
            Get
                Return privateThrowOnInvalidPropertyName
            End Get
            Set(ByVal value As Boolean)
                privateThrowOnInvalidPropertyName = value
            End Set
        End Property

        <Conditional("DEBUG"), DebuggerStepThrough()> _
        Public Sub VerifyPropertyName(ByVal propertyName As String)
            ' Verify that the property name matches a real,  
            ' public, instance property on this object.
            If TypeDescriptor.GetProperties(Me)(propertyName) Is Nothing Then
                Dim msg As String = "Invalid property name: " & propertyName


                If Me.ThrowOnInvalidPropertyName Then
                    Throw New Exception(msg)
                Else
                    Debug.Fail(msg)
                End If
            End If
        End Sub

    End Class

End Namespace