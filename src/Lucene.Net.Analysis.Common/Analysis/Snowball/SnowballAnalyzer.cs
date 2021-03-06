﻿using System;

namespace org.apache.lucene.analysis.snowball
{

	/*
	 * Licensed to the Apache Software Foundation (ASF) under one or more
	 * contributor license agreements.  See the NOTICE file distributed with
	 * this work for additional information regarding copyright ownership.
	 * The ASF licenses this file to You under the Apache License, Version 2.0
	 * (the "License"); you may not use this file except in compliance with
	 * the License.  You may obtain a copy of the License at
	 *
	 *     http://www.apache.org/licenses/LICENSE-2.0
	 *
	 * Unless required by applicable law or agreed to in writing, software
	 * distributed under the License is distributed on an "AS IS" BASIS,
	 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	 * See the License for the specific language governing permissions and
	 * limitations under the License.
	 */

	using org.apache.lucene.analysis;
	using LowerCaseFilter = org.apache.lucene.analysis.core.LowerCaseFilter;
	using StopFilter = org.apache.lucene.analysis.core.StopFilter;
	using EnglishPossessiveFilter = org.apache.lucene.analysis.en.EnglishPossessiveFilter;
	using org.apache.lucene.analysis.standard;
	using TurkishLowerCaseFilter = org.apache.lucene.analysis.tr.TurkishLowerCaseFilter;
	using CharArraySet = org.apache.lucene.analysis.util.CharArraySet;
	using Version = org.apache.lucene.util.Version;

	/// <summary>
	/// Filters <seealso cref="StandardTokenizer"/> with <seealso cref="StandardFilter"/>, {@link
	/// LowerCaseFilter}, <seealso cref="StopFilter"/> and <seealso cref="SnowballFilter"/>.
	/// 
	/// Available stemmers are listed in org.tartarus.snowball.ext.  The name of a
	/// stemmer is the part of the class name before "Stemmer", e.g., the stemmer in
	/// <seealso cref="org.tartarus.snowball.ext.EnglishStemmer"/> is named "English".
	/// 
	/// <para><b>NOTE</b>: This class uses the same <seealso cref="Version"/>
	/// dependent settings as <seealso cref="StandardAnalyzer"/>, with the following addition:
	/// <ul>
	///   <li> As of 3.1, uses <seealso cref="TurkishLowerCaseFilter"/> for Turkish language.
	/// </ul>
	/// </para> </summary>
	/// @deprecated (3.1) Use the language-specific analyzer in modules/analysis instead. 
	/// This analyzer will be removed in Lucene 5.0 
	[Obsolete("(3.1) Use the language-specific analyzer in modules/analysis instead.")]
	public sealed class SnowballAnalyzer : Analyzer
	{
	  private string name;
	  private CharArraySet stopSet;
	  private readonly Version matchVersion;

	  /// <summary>
	  /// Builds the named analyzer with no stop words. </summary>
	  public SnowballAnalyzer(Version matchVersion, string name)
	  {
		this.name = name;
		this.matchVersion = matchVersion;
	  }

	  /// <summary>
	  /// Builds the named analyzer with the given stop words. </summary>
	  public SnowballAnalyzer(Version matchVersion, string name, CharArraySet stopWords) : this(matchVersion, name)
	  {
		stopSet = CharArraySet.unmodifiableSet(CharArraySet.copy(matchVersion, stopWords));
	  }

	  /// <summary>
	  /// Constructs a <seealso cref="StandardTokenizer"/> filtered by a {@link
	  ///    StandardFilter}, a <seealso cref="LowerCaseFilter"/>, a <seealso cref="StopFilter"/>,
	  ///    and a <seealso cref="SnowballFilter"/> 
	  /// </summary>
	  public override TokenStreamComponents createComponents(string fieldName, Reader reader)
	  {
		Tokenizer tokenizer = new StandardTokenizer(matchVersion, reader);
		TokenStream result = new StandardFilter(matchVersion, tokenizer);
		// remove the possessive 's for english stemmers
		if (matchVersion.onOrAfter(Version.LUCENE_31) && (name.Equals("English") || name.Equals("Porter") || name.Equals("Lovins")))
		{
		  result = new EnglishPossessiveFilter(result);
		}
		// Use a special lowercase filter for turkish, the stemmer expects it.
		if (matchVersion.onOrAfter(Version.LUCENE_31) && name.Equals("Turkish"))
		{
		  result = new TurkishLowerCaseFilter(result);
		}
		else
		{
		  result = new LowerCaseFilter(matchVersion, result);
		}
		if (stopSet != null)
		{
		  result = new StopFilter(matchVersion, result, stopSet);
		}
		result = new SnowballFilter(result, name);
		return new TokenStreamComponents(tokenizer, result);
	  }
	}

}