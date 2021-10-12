/* Creating a comment node */

Func<string, CxList> createCommentNode = (text) => {
	Comment root = new Comment();
	Comment comment = new Comment(text, text, root);
	comment.ResolveShortName(text);
	CxList ret = All.NewCxList();
	ret.Add(comment.NodeId, comment);
	return ret;
	};

/* Find all method invoke expr */
CxList routes = Find_ArrayInitializer().CxSelectElements<ArrayInitializer>(x => x.InitialValues).FindByType<MethodInvokeExpr>();

foreach (CxList route in routes)
{
	/* run throw all method invoke expr and filter the urls, paths and re_paths  */
	if ((route == route.FindByName("url")) || (route == route.FindByName("path")) || (route == route.FindByName("re_path")))
	{
		CxList path = route;
		/* Get all the data for each route */
		CxList allUrls_andRoutes = All.GetByAncs(route);
	
		/* Extracting the data */
		CxList endpoint = allUrls_andRoutes.FindByType<StringLiteral>(); 
		/* StringLiteral returns the endpoints + the value of the "name" so we will need to use book in order to get 
		the endpoint in the first run, and then the value of "name" later */
		CxList include = allUrls_andRoutes.FindByName("include");
		CxList member = allUrls_andRoutes.FindByType<MemberAccess>();
		CxList UnknownRef = allUrls_andRoutes.FindByType(typeof(UnknownReference));
		bool getendpoint = true;
		
		/* Run throw each endpoint */
		foreach (CxList end in endpoint)
		{
			if (getendpoint == true)
			{
				path = path.ConcatenatePath(createCommentNode("endpoint"), false);
				path = path.ConcatenatePath(end, false);
				/* Check param type, in case there is one */
				if (end.GetName().Contains("<str:") == true)
				{
					path = path.ConcatenatePath(createCommentNode("param type"), false);
					path = path.ConcatenatePath(createCommentNode("str"), false);
				}
				else 
					if (end.GetName().Contains("<float:") == true)
				{
					path = path.ConcatenatePath(createCommentNode("param type"), false);
					path = path.ConcatenatePath(createCommentNode("float"), false);
				}
			
				else 
					if (end.GetName().Contains("<path:") == true)
				{
					path = path.ConcatenatePath(createCommentNode("param type"), false);
					path = path.ConcatenatePath(createCommentNode("path"), false);
				}
				else 
					if (end.GetName().Contains("<int:") == true)
				{
					path = path.ConcatenatePath(createCommentNode("param type"), false);
					path = path.ConcatenatePath(createCommentNode("int"), false);
				}
			}
			/* Will happen after we finished iterating the endpoint */
			if (getendpoint == false)
			{
				if (include.Count > 0){ /* There is use of include function */
					path = path.ConcatenatePath(createCommentNode("include"), false);
					path = path.ConcatenatePath(end, false);
				}
				else /* There is not a use of include function (defualt use - "name") */
				{
					path = path.ConcatenatePath(createCommentNode("name"), false);
					path = path.ConcatenatePath(end, false);
					path = path.ConcatenatePath(createCommentNode("view"), false);
				}
			}
			/* Setting this bool to flase so in the next run (after finishing getting the endpoint) we will check include/name variablas */
			getendpoint = false;
			
		}
			/* Getting the view as "views.index" for example */		
			if (member.Count > 0)
			{
				foreach (CxList mem in member)
				{
					path = path.ConcatenatePath(mem, false);
				}
			}
			else 
				/* Getting the view as "user.login" for example after it got imported (from loginas.views import user_login) */	
				if (UnknownRef.Count > 0)
			{
				foreach (CxList unknown in UnknownRef)
				{
					path = path.ConcatenatePath(unknown, false);
				}
			}
	
		result.Add(path);
	}
}