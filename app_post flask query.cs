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

CxList appRoute = customAttributes.FindByCustomAttribute(flaskAppVar + ".post");
appRoute.Add(customAttributes.FindByCustomAttribute(flaskAppVar + ".get"));
appRoute.Add(customAttributes.FindByCustomAttribute(flaskAppVar + ".put"));
appRoute.Add(customAttributes.FindByCustomAttribute(flaskAppVar + ".delete"));
appRoute.Add(customAttributes.FindByCustomAttribute(flaskAppVar + ".patch"));

foreach (CxList ar in appRoute) {
	
	CxList path = ar;
	CxList allUrls_andRoutes = All.GetByAncs(ar);
	
	/* urls of decorations */
	CxList Urls = ar.CxSelectElements<CustomAttribute>(x => x.Parameters).FindByType<StringLiteral>();
	path = path.ConcatenatePath(createCommentNode("Url"), false);
	path = path.ConcatenatePath(Urls, false);

	CxList appRouteMethods = allUrls_andRoutes.FindByShortName("defaults");
	path = path.ConcatenatePath(createCommentNode("**options"), false);
	path = path.ConcatenatePath(appRouteMethods, false);
	

	/* HTTP type of decorations */
	
	CxList getHttpType = ar.CxSelectElements<CustomAttribute>(x => x.Parameters).FindByType<AssignExpr>();
	return getHttpType;
	
	
	
	/*foreach (CxList type in getHttpType){
			
	path = path.ConcatenatePath(type, false);
	}*/
	
	path = path.ConcatenatePath(createCommentNode("HTTP types"), false);
	path = path.ConcatenatePath(ar, false);
	
	/* input parameters of decorations */
	CxList input_param = All.GetParameters(ar.GetAncOfType<MethodDecl>());
	
	if (input_param.Count > 0){
		
		cxLog.WriteDebugMessage(input_param);
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

