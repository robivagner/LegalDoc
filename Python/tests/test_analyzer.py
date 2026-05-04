# tests/test_analyzer.py
import pytest
from unittest.mock import AsyncMock, MagicMock
from analyzer import DocumentAnalyzer

@pytest.mark.asyncio
async def test_analyze_cleans_json_markdown(mocker):
    # Arrange
    analyzer = DocumentAnalyzer()

    # Simulăm un răspuns de la Gemini care vine înfășurat în block-uri de markdown
    mock_response = MagicMock()
    mock_response.text = '```json {"summary": "test", "clauses": "test", "risks": "test"} ```'

    # Mock-uim apelul asincron către Google SDK
    mocker.patch.object(analyzer.client.aio.models, 'generate_content', new_callable=AsyncMock, return_value=mock_response)

    # Act
    result = await analyzer.analyze("Acesta este un document de test destul de lung.")

    # Assert
    assert result["summary"] == "test"
    assert "summary" in result