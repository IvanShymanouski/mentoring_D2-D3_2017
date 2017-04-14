<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
    xmlns:ts="http://library.by/catalog"
>
  <xsl:output method="html" indent="yes"/>
  
  <xsl:key name="group" match="ts:book" use="ts:genre"/>

  <xsl:template match="ts:catalog">
    <html>
      <head>
        <title>Grouped books</title>
      </head>
      <body>
        <h2>
          <xsl:value-of select="ts:GetCurrentDate()"/>
        </h2>
        <xsl:apply-templates select="ts:book[generate-id(.) = generate-id(key('group', ts:genre))]"/>
        <h2>
          Total count: <xsl:value-of select="count(ts:book)"/>
        </h2>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="ts:book">
    <h2>
      Genre: <xsl:value-of select="ts:genre"/>
    </h2>
    <xsl:for-each select="key('group', ts:genre)">
      <p>
        Author :  <xsl:value-of select="ts:author"/><br/>
        Title : <xsl:value-of select="ts:title"/><br/>
        Publish date : <xsl:value-of select="ts:publish_date"/><br/>
        Registration date : <xsl:value-of select="ts:registration_date"/><br/>
      </p>
    </xsl:for-each>
    <h3>
      Count: <xsl:value-of select="count(key('group', ts:genre))"/>
    </h3>
  </xsl:template>
  
  <msxsl:script language="CSharp" implements-prefix="ts">
    public string GetCurrentDate()
    {
      return DateTime.Now.ToString();
    }
  </msxsl:script>

</xsl:stylesheet>
