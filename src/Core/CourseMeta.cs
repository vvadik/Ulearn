using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Ulearn.Common.Extensions;

namespace Ulearn.Core
{
	// Это данные, которые добавляются на сервере а автору курса никогда не видны
	[DataContract]
	public class CourseMeta
	{
		[NotNull]
		[DataMember]
		public CourseVersionToken Version;
	}

	[DataContract]
	public class CourseVersionToken : IEqualityComparer<CourseVersionToken>
	{
		public CourseVersionToken()
		{
		}

		public CourseVersionToken(Guid version)
		{
			Version = version;
		}

		public CourseVersionToken(DateTime tempCourseLoadingTime)
		{
			LoadingTime = tempCourseLoadingTime;
		}

		[DataMember(EmitDefaultValue = false)]
		public Guid? Version { get; set; } // null для временных курсов и проверяемых курсов, для которых еще нет версии

		[DataMember(EmitDefaultValue = false)]
		public DateTime? LoadingTime { get; set; } // То, что вместо версии во временном курсе, дата LoadingTime

		public bool IsTempCourse()
		{
			return LoadingTime != null;
		}

		public override bool Equals(object obj)
		{
			return Equals(this, obj as CourseVersionToken);
		}

		public override int GetHashCode()
		{
			return GetHashCode(this);
		}

		public bool Equals(CourseVersionToken x, CourseVersionToken y)
		{
			if (ReferenceEquals(x, y))
				return true;
			if (ReferenceEquals(x, null))
				return false;
			if (ReferenceEquals(y, null))
				return false;
			if (x.GetType() != y.GetType())
				return false;
			return Nullable.Equals(x.Version, y.Version) && Nullable.Equals(x.LoadingTime, y.LoadingTime);
		}

		public int GetHashCode(CourseVersionToken obj)
		{
			if (obj.Version != null)
				return obj.Version.GetHashCode();
			if (obj.LoadingTime != null)
				return obj.LoadingTime.GetHashCode();
			return 0;
		}

		public static bool operator ==(CourseVersionToken x, CourseVersionToken y)
		{
			if (x is null)
				return y is null;
			return x.Equals(y);
		}

		public static bool operator !=(CourseVersionToken x, CourseVersionToken y)
		{
			return !(x == y);
		}

		public override string ToString()
		{
			if (Version != null)
				return Version.ToString();
			if (LoadingTime != null)
				return LoadingTime.Value.ToSortable();
			return "";
		}
	}
}