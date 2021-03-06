if ENABLE_DOTNET
TARGET = $(ASSEMBLY) $(ASSEMBLY).config
pkgconfigdir=$(libdir)/pkgconfig
pkgconfig_DATA=gtk-dotnet-3.0.pc
else
TARGET =
endif

SNK = $(top_srcdir)/gtk-sharp.snk
ASSEMBLY = $(ASSEMBLY_NAME).dll
ASSEMBLY_NAME = gtk-dotnet
noinst_DATA = $(ASSEMBLY) $(POLICY_ASSEMBLIES)
CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb $(POLICY_ASSEMBLIES) $(POLICY_CONFIGS)
DISTCLEANFILES = $(ASSEMBLY).config

POLICY_ASSEMBLIES = $(addsuffix .$(ASSEMBLY), $(addprefix policy., $(POLICY_VERSIONS)))
POLICY_CONFIGS = $(addsuffix .config, $(addprefix policy., $(POLICY_VERSIONS)))

references = ../glib/glib-sharp.dll ../gio/gio-sharp.dll ../cairo/cairo-sharp.dll ../pango/pango-sharp.dll ../gdk/gdk-sharp.dll
build_references = $(addprefix -r:, $(references)) -r:System.Drawing.dll

sources =		 			\
	Graphics.cs

build_sources = $(addprefix $(srcdir)/, $(sources)) $(top_builddir)/AssemblyInfo.cs

EXTRA_DIST =				\
	$(sources)			\
	$(ASSEMBLY).config.in		\
	gtk-dotnet-3.0.pc.in

$(ASSEMBLY): $(build_sources) $(references) $(SNK)
	@rm -f $(ASSEMBLY).mdb
	$(CSC) $(CSFLAGS) -keyfile:$(SNK) -nowarn:0169,0612,0618 -out:$(ASSEMBLY) -target:library $(build_references) $(build_sources)

policy.%.config: $(top_builddir)/policy.config
	sed -e "s/@ASSEMBLY_NAME@/$(ASSEMBLY_NAME)/" -e "s/@POLICY@/$*/" $(top_builddir)/policy.config > $@

$(POLICY_ASSEMBLIES) : policy.%.$(ASSEMBLY): policy.%.config $(SNK)
	$(AL) -link:policy.$*.config -out:$@ -keyfile:$(SNK)

install-data-local:
	@if test -n '$(TARGET)'; then							\
	  echo "$(GACUTIL) -i $(ASSEMBLY) -f $(GACUTIL_FLAGS)";				\
	  $(GACUTIL) -i $(ASSEMBLY) -f $(GACUTIL_FLAGS) || exit 1;			\
	  if test -n '$(POLICY_VERSIONS)'; then						\
	    for i in $(POLICY_VERSIONS); do						\
	      echo "$(GACUTIL) -i policy.$$i.$(ASSEMBLY) -f $(GACUTIL_FLAGS)";		\
	      $(GACUTIL) -i policy.$$i.$(ASSEMBLY) -f $(GACUTIL_FLAGS) || exit 1;	\
	    done									\
	  fi										\
	fi

uninstall-local:
	@if test -n '$(TARGET)'; then							\
	  echo "$(GACUTIL) -u $(ASSEMBLY_NAME) $(GACUTIL_FLAGS)";			\
	  $(GACUTIL) -u $(ASSEMBLY_NAME) $(GACUTIL_FLAGS) || exit 1;			\
	  if test -n '$(POLICY_VERSIONS)'; then						\
	    for i in $(POLICY_VERSIONS); do						\
	      echo "$(GACUTIL) -u policy.$$i.$(ASSEMBLY_NAME) $(GACUTIL_FLAGS)";	\
	      $(GACUTIL) -u policy.$$i.$(ASSEMBLY_NAME) $(GACUTIL_FLAGS) || exit 1;	\
	    done									\
	  fi										\
	fi

