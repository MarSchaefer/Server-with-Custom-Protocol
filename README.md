Dies ist eine in bearbeitung befindliche Client/Server-Anwendung, geschrieben in C#.

Meine Motivation war es, ein eigenes, effizientes Übertragungsprotokoll zu schreiben, um Packete schnell und sicher
verschicken zu können. Hierbei werden die Packete, so wie sie im Speicher stehen, als Gruppe zusammengefasst in
mehrere Packete aufgeteilt, und auf der jeweiligen Gegenseite genau so wieder zusammengebaut. Dabei kann dann
der Speicher direkt als Datentyp von Struct<T> interpretiert werden.

Anbei ist schon ein tokenbasiertes Anmeldeverfahren mittels Challenge-Response-Authentifizierung implementiert.
Die Anmeldung ist zusätzlich mittels RSA-Verschlüsselung gesichert.

In Zukunft, wenn ich Zeit habe, werde ich noch die tokenbasierte AES-Verschlüsselung implementieren, nachdem sich
der Benutzer momentan schon Anmelden kann und ein Token zugeteilt bekommt.


