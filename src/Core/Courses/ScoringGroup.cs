using System.Xml.Serialization;

namespace Ulearn.Core.Courses
{
	public class ScoringGroup
	{
		private const bool DefaultCanBeSetByInstructor = false;
		private const int DefaultMaxAdditionalScore = 10;
		private const bool DefaultEnabledForEveryone = false;

		[XmlAttribute("id")]
		public string Id { get; set; }

		[XmlAttribute("abbr")]
		public string Abbreviation { get; set; }

		[XmlAttribute("description")]
		public string Description { get; set; }

		/* Hack to handle empty bool and integer attributes,
		 * because standard XmlSerializer doesn't work with nullable (i.e. int? and bool?) fields */
		[XmlAttribute("set_by_instructor")]
		public string _canBeSetByInstructor;

		[XmlIgnore]
		public bool CanBeSetByInstructor
		{
			get
			{
				if (string.IsNullOrEmpty(_canBeSetByInstructor) || _canBeSetByInstructor.Trim().Length == 0)
					return DefaultCanBeSetByInstructor;

				return bool.TryParse(_canBeSetByInstructor, out bool value) ? value : DefaultCanBeSetByInstructor;
			}
			set => _canBeSetByInstructor = value.ToString();
		}

		[XmlIgnore]
		public bool IsCanBeSetByInstructorSpecified => !string.IsNullOrEmpty(_canBeSetByInstructor);

		[XmlAttribute("max_additional_score")]
		public string _maxAdditionalScore { get; set; }

		[XmlIgnore]
		public int MaxAdditionalScore
		{
			get
			{
				if (string.IsNullOrEmpty(_maxAdditionalScore) || _maxAdditionalScore.Trim().Length == 0)
					return DefaultMaxAdditionalScore;

				int result;
				return int.TryParse(_maxAdditionalScore, out result) ? result : DefaultMaxAdditionalScore;
			}
			set => _maxAdditionalScore = value.ToString();
		}

		[XmlIgnore]
		/* Calculates automatically by slides's scores */
		public int MaxNotAdditionalScore { get; set; }

		[XmlIgnore]
		public bool IsMaxAdditionalScoreSpecified => !string.IsNullOrEmpty(_maxAdditionalScore);

		[XmlAttribute("enable_for_everyone")]
		public string _enabledForEveryone;

		[XmlIgnore]
		public bool EnabledForEveryone
		{
			get
			{
				if (string.IsNullOrEmpty(_enabledForEveryone) || _enabledForEveryone.Trim().Length == 0)
					return DefaultEnabledForEveryone;

				return bool.TryParse(_enabledForEveryone, out bool value) ? value : DefaultEnabledForEveryone;
			}
			set => _enabledForEveryone = value.ToString();
		}

		[XmlIgnore]
		public bool IsEnabledForEveryoneSpecified => !string.IsNullOrEmpty(_enabledForEveryone);

		[XmlText]
		public string Name { get; set; }

		public void CopySettingsFrom(ScoringGroup otherScoringGroup)
		{
			_canBeSetByInstructor = string.IsNullOrEmpty(_canBeSetByInstructor) ? otherScoringGroup._canBeSetByInstructor : _canBeSetByInstructor;
			_maxAdditionalScore = string.IsNullOrEmpty(_maxAdditionalScore) ? otherScoringGroup._maxAdditionalScore : _maxAdditionalScore;
			_enabledForEveryone = string.IsNullOrEmpty(_enabledForEveryone) ? otherScoringGroup._enabledForEveryone : _enabledForEveryone;
			Abbreviation = Abbreviation ?? otherScoringGroup.Abbreviation;
			Name = string.IsNullOrEmpty(Name) ? otherScoringGroup.Name : Name;
			Description = string.IsNullOrEmpty(Description) ? otherScoringGroup.Description : Description;
		}
	}
}