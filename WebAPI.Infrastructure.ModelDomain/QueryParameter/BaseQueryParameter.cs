using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WebAPI.Infrastructure.DomainModel;

namespace WebAPI.Infrastructure.ModelDomain.QueryParameter
{
    public abstract class BaseQueryParameter:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private const int DefaultPageSize = 10;
        private const int DefaultMaxPageIndex = 100;

        private int _pageIndex;
        public int PageIndex
        {
            get => _pageIndex;
            set => _pageIndex = value >= 0 ? value : 0;
        }

        private int _pageSize = DefaultPageSize;
        public virtual int PageSize
        {
            get => _pageSize;
            set => SetField(ref _pageSize, value);
        }

        private string _orderBy;
        public string OrderBy
        {
            get => _orderBy;
            set => _orderBy = value ?? nameof(BaseEntity.Id);
        }

        private int _maxPageSize = DefaultMaxPageIndex;
        protected internal virtual int MaxPageSize
        {
            get => _maxPageSize;
            set => SetField(ref _maxPageSize, value);
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            if(propertyName==nameof(PageSize) || propertyName==nameof(MaxPageSize))
                SetPageSize();
            return true;
        }

        private void SetPageSize()
        {
            if (_maxPageSize <= 0)
                _maxPageSize = DefaultMaxPageIndex;
            if (_pageSize <= 0)
                _pageSize = DefaultPageSize;
            _pageSize = _pageSize > _maxPageSize ? _maxPageSize : _pageSize;
        }
        
        public string Fields { get; set; }
    }
}