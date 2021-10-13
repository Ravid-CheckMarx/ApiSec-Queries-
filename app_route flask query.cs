/* Creating a node */
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
/* Regex for getting the render template */
CxList render_source = All.FindByRegexExt(@"(?<=render_template\([''""])(\w+\.[a-z]{3,})(?=[''""],?.+\))");

CxList appRoute = customAttributes.FindByCustomAttribute(flaskAppVar + ".route");


	
	// Flask @app.route decorator
CxList route = methods.FindByShortName("route");
CxList flk = methods.FindByShortName("Flask").GetAssignee();
CxList app = All.FindAllReferences(flk).GetMembersOfTarget();

/* Running throw each route */
foreach (CxList ar in appRoute) 
{
	CxList path = ar;
	CxList allUrls_andRoutes = All.GetByAncs(ar);
	/* Extracting the method of a route*/
	CxList fun = All.GetMethod(ar);
	
	/* urls of decorations */
	CxList Urls = ar.CxSelectElements<CustomAttribute>(x => x.Parameters).FindByType<StringLiteral>();
	path = path.ConcatenatePath(createCommentNode("Url"), false);
	path = path.ConcatenatePath(Urls, false);
	
	/*string abc = Urls.GetName();
	foreach (CxList b in abc)
	{
		string[] bb = b.Split('<');
		cxLog.WriteDebugMessage(bb);
	}*/	

	/* HTTP type of decorations */
	
	/* Getch the array of the HTTP types */
	CxList getHttpType = Find_ArrayInitializer().GetByAncs(ar).CxSelectElements<ArrayInitializer>(x => x.InitialValues);
	
	path = path.ConcatenatePath(createCommentNode("HTTP types"), false);
	/* Fetch only routes that has the method=[] in it */
	CxList app_routes_methods = Find_ArrayInitializer().GetByAncs(ar);
	if (app_routes_methods.Count > 0)
	{
		foreach (CxList type in getHttpType)
		{
			/*Returning all HTTP types */
			path = path.ConcatenatePath(type, false);
		}
	}
	else 
		/* In case no types mentioned, the defualt is GET */
		path = path.ConcatenatePath(createCommentNode("GET"), false);
	
	
	path = path.ConcatenatePath(createCommentNode("method"), false);
	/* Return the method attached to each route */
	path = path.ConcatenatePath(fun, false);
	
	
	/* input parameters of decorations */
	CxList input_param = All.GetParameters(ar.GetAncOfType<MethodDecl>());
	
	if (input_param.Count > 0)
	{
		if (Urls.GetName().Contains("<int:") == true)
		{
			path = path.ConcatenatePath(createCommentNode("param type"), false);
			path = path.ConcatenatePath(createCommentNode("int"), false);
		}
		else 
			if (Urls.GetName().Contains("<float:") == true)
		{
			path = path.ConcatenatePath(createCommentNode("param type"), false);
			path = path.ConcatenatePath(createCommentNode("float"), false);
		}
			
		else 
			if (Urls.GetName().Contains("<path:") == true)
		{
			path = path.ConcatenatePath(createCommentNode("param type"), false);
			path = path.ConcatenatePath(createCommentNode("path"), false);
		}
		else 
			if (Urls.GetName().Contains("<uuid:") == true)
		{
			path = path.ConcatenatePath(createCommentNode("param type"), false);
			path = path.ConcatenatePath(createCommentNode("int"), false);
		}	
		else
		{
			path = path.ConcatenatePath(createCommentNode("param type"), false);
			path = path.ConcatenatePath(createCommentNode("str"), false);
		}
			
		
		path = path.ConcatenatePath(createCommentNode("input parameter name"), false);
		foreach (CxList par in input_param)
			path = path.ConcatenatePath(par, false);
			
	}
	else{
		path = path.ConcatenatePath(createCommentNode("input parameters"), false);
		path = path.ConcatenatePath(createCommentNode("no parameters"), false);
	}
	/* trying to fetch the rendered page that the fuction returns (in case there is one) */
/*	var viewCallLine = Find_ViewCalls().GetByAncs(ar.GetAncOfType<MethodDecl>()).CxSelectElementValues<ViewCall,int>(x => x.LinePragma.Line && x.LinePragma.Filename == ar.LinePragma.Filename).FirstOrDefault();

	if (viewCallLine != null)
	{
		result.Add(render_source.FindByPosition(viewCallLine));
	}
*/
	result.Add(path);
}


