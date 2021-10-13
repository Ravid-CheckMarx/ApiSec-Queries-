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
CxList render_source = All.FindByRegexExt(@"(?<=render_template\([''""])(\w+\.[a-z]{3,})(?=[''""],?.+\))");

CxList appRoute = customAttributes.FindByCustomAttribute(flaskAppVar + ".route");


	
	// Flask @app.route decorator
CxList route = methods.FindByShortName("route");
CxList flk = methods.FindByShortName("Flask").GetAssignee();
CxList app = All.FindAllReferences(flk).GetMembersOfTarget();
	
/*CxList parameter = All.GetParameters(fun);*/

/*CxList app_routes_methods = Find_ArrayInitializer().GetByAncs(appRoute);*/

foreach (CxList ar in appRoute) {
	
	
	CxList path = ar;
	CxList allUrls_andRoutes = All.GetByAncs(ar);
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

	CxList appRouteMethods = allUrls_andRoutes.FindByShortName("methods");
	
	/* HTTP type of decorations */
	
	CxList getHttpType = Find_ArrayInitializer().GetByAncs(ar).CxSelectElements<ArrayInitializer>(x => x.InitialValues);
	
	
	path = path.ConcatenatePath(createCommentNode("HTTP types"), false);
	CxList app_routes_methods = Find_ArrayInitializer().GetByAncs(ar);
	if (app_routes_methods.Count > 0)
	{
		foreach (CxList type in getHttpType)
		{
			path = path.ConcatenatePath(type, false);
		}
	}
	else 
		path = path.ConcatenatePath(createCommentNode("GET"), false);
	
	
	path = path.ConcatenatePath(createCommentNode("method"), false);
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
	
	result.Add(path);
}


