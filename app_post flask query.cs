Func<string, CxList> createCommentNode = (text) => {
	Comment root = new Comment();
	Comment comment = new Comment(text, text, root);
	comment.ResolveShortName(text);
	CxList ret = All.NewCxList();
	ret.Add(comment.NodeId, comment);
	return ret;
	};


CxList customAttributes = Find_CustomAttribute();
CxList methods = Find_Methods();
CxList flaskApp = Find_Methods_By_Import("flask", new string[] {"Flask"}).GetAssignee();
string flaskAppVar = flaskApp.GetName();

/* Triggering every possible option */
CxList appRoute = customAttributes.FindByCustomAttribute(flaskAppVar + ".post");
appRoute.Add(customAttributes.FindByCustomAttribute(flaskAppVar + ".get"));
appRoute.Add(customAttributes.FindByCustomAttribute(flaskAppVar + ".put"));
appRoute.Add(customAttributes.FindByCustomAttribute(flaskAppVar + ".delete"));
appRoute.Add(customAttributes.FindByCustomAttribute(flaskAppVar + ".patch"));

foreach (CxList ar in appRoute) {
	
	CxList path = ar;
	CxList allUrls_andRoutes = All.GetByAncs(ar);
	CxList fun = All.GetMethod(ar);
	
	/* urls of decorations */
	CxList Urls = ar.CxSelectElements<CustomAttribute>(x => x.Parameters).FindByType<StringLiteral>();
	path = path.ConcatenatePath(createCommentNode("Url"), false);
	path = path.ConcatenatePath(Urls, false);

	CxList appRouteMethods = allUrls_andRoutes.FindByShortName("defaults");
	path = path.ConcatenatePath(createCommentNode("**options"), false);
	path = path.ConcatenatePath(appRouteMethods, false);
	

	/* HTTP type of decorations */
	
	CxList getHttpType = ar.CxSelectElements<CustomAttribute>(x => x.Parameters).FindByType<AssignExpr>();
	
	path = path.ConcatenatePath(createCommentNode("HTTP types"), false);
	path = path.ConcatenatePath(ar, false);
	
	path = path.ConcatenatePath(createCommentNode("method"), false);
	/* Return the method attached to each route */
	path = path.ConcatenatePath(fun, false);
	
	/* input parameters of decorations */
	CxList input_param = All.GetParameters(ar.GetAncOfType<MethodDecl>());
	
	if (input_param.Count > 0)
	{
		
		string str_url = Urls.GetName();
		char[] delimiterChars = {'/'};
		string[] words = str_url.Split(delimiterChars);

		foreach (var word in words)
		{
			/* Check if we reached the <type:parameter name> pair in the URL */
			if (word.StartsWith("<"))
			{
				/* Split between the type and the param name */
				string[] par_type_and_name = word.Split(':');
				foreach (var par in par_type_and_name)
				{
					/* Check if this is the type */
					if (par.StartsWith("<"))
					{
						if (word.Contains(":") == true)
						{
							/* Remove unnessecary chars */
							var trimmed_par = par.Trim('<');
							path = path.ConcatenatePath(createCommentNode("param type"), false);
							path = path.ConcatenatePath(createCommentNode(trimmed_par), false);
						}	
					}
					/* Check if this is the name */
					if (par.EndsWith(">"))
					{
						/* Remove unnessecary chars */
						var trimmed_par = par.Trim('<', '>');
						path = path.ConcatenatePath(createCommentNode("param name"), false);
						path = path.ConcatenatePath(createCommentNode(trimmed_par), false);
					}
				}
			}
		}
		/* In case there are no paramters in the URL, but there are in the method */
		if (Urls.GetName().Contains(":") == false)
		{
			/* Running on each one of the parameters */
			foreach (CxList parameter in input_param)
			{
				path = path.ConcatenatePath(createCommentNode("param type"), false);
				path = path.ConcatenatePath(createCommentNode("str"), false);
				path = path.ConcatenatePath(createCommentNode("input parameter name"), false);
				path = path.ConcatenatePath(parameter, false);
			}
		}
	}
		
	else
	{
		path = path.ConcatenatePath(createCommentNode("input parameters"), false);
		path = path.ConcatenatePath(createCommentNode("no parameters"), false);
	}
	
	result.Add(path);
}
