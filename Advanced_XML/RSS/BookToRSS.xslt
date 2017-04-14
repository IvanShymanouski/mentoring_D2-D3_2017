<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
    xmlns:ts="http://library.by/catalog"
>
  <xsl:output method="xml" indent="yes"/>
  
  <xsl:template match="/">
    <rss version="2.0">
      <channel>
        <title>RSS Books</title>
        <link>http://library.by/</link>
        <xsl:apply-templates select="ts:catalog/ts:book"/>
      </channel>
    </rss>
  </xsl:template>

  <xsl:template match="ts:catalog/ts:book">
    <item>
      <title>
        <xsl:value-of select="ts:title"/>
      </title>
      <genre>
        <xsl:value-of select="ts:genre"/>
      </genre>
      <description>
        <xsl:value-of select="ts:description"/>
      </description>
        <xsl:choose>
          <xsl:when test="ts:genre = 'Computer' and ts:isbn">
            <link>
               http://my.safaribooksonline.com/<xsl:value-of select="ts:isbn"/>/
            </link>
          </xsl:when>
        </xsl:choose>
      <pubDate>
        <xsl:value-of select="ts:registration_date"/>
      </pubDate>
    </item>
  </xsl:template>
</xsl:stylesheet>
